"""Compile extracted production methods against narrow ESPHome boundary stubs."""
import json
import os
from pathlib import Path
import shutil
import subprocess
import tempfile
import unittest
from host.source import ROOT, generate

class NativeHarness(unittest.TestCase):
    kinds = ()
    @classmethod
    def setUpClass(cls):
        cls.tmp = tempfile.TemporaryDirectory(prefix='iq2020-host-')
        cls.addClassCleanup(cls.tmp.cleanup)
        cls.folder = Path(cls.tmp.name)
        compiler = os.environ.get('CXX') or shutil.which('clang++') or shutil.which('g++')
        if not compiler:
            raise RuntimeError('Install clang++ or g++; tests may not silently skip')
        cls.executables = {}
        for name in cls.kinds:
            kind = name.removeprefix('legacy_')
            component = ROOT / 'components' / ('iq2020-legacy' if name.startswith('legacy_') else 'iq2020')
            cpp = cls.folder / f'{name}.cpp'
            cpp.write_text(generate(kind, component))
            exe = cls.folder / name
            result = subprocess.run([compiler, '-std=c++17', '-g', '-O0', '-fsanitize=address,undefined', '-fno-omit-frame-pointer', str(cpp), '-o', str(exe)], capture_output=True, text=True)
            if result.returncode:
                raise RuntimeError(result.stderr)
            cls.executables[name] = exe
        if 'rx' in cls.kinds:
            fixture = json.loads((ROOT / 'tests/fixtures/iq-status.json').read_text())
            payload = bytes.fromhex(fixture['payload_hex'])
            packet = bytes([0x1c, fixture['destination'], fixture['source'], len(payload), fixture['operation']]) + payload
            cls.packet = (packet + bytes([(sum(packet[1:]) & 255) ^ 255])).hex()
            cls.expected = fixture['expected']
        cls.results = {}
        if 'controls' in cls.kinds:
            for mode in ['zero','normal','retry','pending','inactive','fan_alias','fan_speed','fan_on','temperature']:
                cls.results[mode] = cls.success('controls', mode)
        cls.crashes = {}
        for key, kind, args in [('overlap','rx',['overlap',getattr(cls,'packet','')]),('title','audio',['title','21']),('artist','audio',['artist','21']),('invalid_id','controls',['invalid_id'])]:
            if kind in cls.kinds:
                result = cls.run_native(kind, *args)
                if result.returncode:
                    raise RuntimeError(result.stderr)
                cls.crashes[key] = result.returncode
    @classmethod
    def run_native(cls, kind, *args):
        env = dict(os.environ, ASAN_OPTIONS='detect_leaks=0', UBSAN_OPTIONS='halt_on_error=1')
        return subprocess.run([str(cls.executables[kind]), *args], capture_output=True, text=True, env=env, timeout=10)
    @classmethod
    def success(cls, kind, *args):
        result = cls.run_native(kind, *args)
        if result.returncode:
            raise RuntimeError(result.stderr)
        return json.loads(result.stdout)
