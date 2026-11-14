"""Portable build matrix with dummy credentials. Never uploads to a real spa."""
import argparse
import json
import hashlib
import os
from pathlib import Path
import subprocess
import sys
import tempfile
import yaml

ROOT = Path(__file__).resolve().parents[2]
WORK = ROOT / 'tests' / '.work'
# ESP-IDF rejects spaces in project/toolchain paths. Keep builds portable even
# when the checkout itself lives in a directory such as "HA Spa Monitor".
BUILD = Path(os.environ.get('IQ2020_BUILD_ROOT', str(Path(tempfile.gettempdir()) / ('iq2020-build-' + hashlib.sha256(str(ROOT).encode()).hexdigest()[:10]))))

def classify(name, stage, result):
    return 'PASS' if result.returncode == 0 else 'FAIL'


def configs():
    chemistry = [{'platform': 'iq2020', 'iq_chlorine': {'name': 'Chlorine'}, 'iq_ph': {'name': 'pH'}, 'iq_hoursleft': {'name': 'IQ hours'}}]
    control = {
        'number': [{'platform': 'iq2020', 'name': 'Salt output', 'datapoint': 5}],
        'switch': [{'platform': 'iq2020', 'name': 'Boost', 'datapoint': 8}],
        'fan': [{'platform': 'iq2020', 'name': 'Jets', 'datapoint': 0, 'speeds': 2}],
        'climate': [{'platform': 'iq2020', 'name': 'Temperature', 'celsius': False}],
    }
    select = [{'platform': 'iq2020', 'name': 'Audio source', 'datapoint': 0}]
    return {
        'minimal': {},
        'chemistry': {'sensor': chemistry},
        'text-only': {'text_sensor': [{'platform': 'iq2020', 'rtc_datetime': {'name': 'Clock'}, 'salt_module_status': {'name': 'Salt status'}}]},
        'salt-sensor': {'sensor': [{'platform': 'iq2020', 'salt_power': {'name': 'Salt reported output'}, 'salt_cartridge_age_days': {'name': 'Cartridge age'}, 'salt_days_since_manual_test': {'name': 'Days since test'}, 'salt_generation_hours': {'name': 'Generation hours'}}]},
        'iq-ph-only': {'sensor': [{'platform': 'iq2020', 'iq_ph': {'name': 'pH'}}]},
        'controls': control,
        'select-without-number': {'select': select},
        'full': {**control, 'sensor': chemistry, 'select': select,
                 'text_sensor': [{'platform': 'iq2020', 'versionstr': {'name': 'Version'}, 'salt_module_status': {'name': 'Salt status'}}],
                 'text': [{'platform': 'iq2020', 'name': 'Song', 'datapoint': 0, 'mode': 'text'}],
                 'button': [{'platform': 'iq2020', 'name': 'Confirm test', 'datapoint': 2}],
                 'binary_sensor': [{'platform': 'iq2020', 'salt_level_confirmed': {'name': 'Confirmed'}}]},
    }


def write_config(name, extra):
    WORK.mkdir(parents=True, exist_ok=True)
    config = {
        'esphome': {'name': f'iqtest-{name}', 'build_path': str(BUILD / name)},
        'esp32': {'board': 'm5stack-atom', 'framework': {'type': 'esp-idf'}},
        'logger': {'baud_rate': 0}, 'api': {},
        'wifi': {'ssid': 'test-only-no-device', 'password': 'test-only-password'},
        'external_components': [{'source': {'type': 'local', 'path': str(ROOT / 'components')}, 'components': ['iq2020']}],
        'uart': {'id': 'spa_bus', 'tx_pin': 26, 'rx_pin': 32, 'baud_rate': 38400},
        'iq2020': {'uart_id': 'spa_bus', 'port': 0, 'active': True},
        **extra,
    }
    path = WORK / f'{name}.yaml'
    path.write_text(yaml.safe_dump(config, sort_keys=False))
    return path


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--stage', choices=['config', 'generate', 'compile'], default='generate')
    parser.add_argument('--case', choices=list(configs()))
    args = parser.parse_args()
    report = []
    for name, extra in configs().items():
        if args.case and args.case != name:
            continue
        path = write_config(name, extra)
        command = [sys.executable, '-m', 'esphome', 'config' if args.stage == 'config' else 'compile', str(path)]
        if args.stage == 'generate':
            command.append('--only-generate')
        env = dict(os.environ)
        env.setdefault('PLATFORMIO_CORE_DIR', str(BUILD / 'platformio'))
        # Keep the exact known-error signatures stable across host locales.
        env['LC_ALL'] = 'C'
        result = subprocess.run(command, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, env=env)
        log = WORK / f'{name}-{args.stage}.log'
        log.write_text(result.stdout)
        status = classify(name, args.stage, result)
        print(f'{name}: {args.stage}: {status} ({log})', flush=True)
        if status == 'FAIL':
            print(result.stdout[-6000:], flush=True)
        report.append({'case': name, 'stage': args.stage, 'status': status, 'exit_code': result.returncode, 'log': log.name})
    report_path = WORK / f'{args.stage}-{args.case or "all"}.json'
    report_path.write_text(json.dumps(report, indent=2) + '\n')
    return int(any(row['status'] in ('FAIL',) for row in report))


if __name__ == '__main__':
    raise SystemExit(main())
