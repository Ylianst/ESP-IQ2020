"""Validate actual external-component schemas with the pinned ESPHome install."""
import importlib
from pathlib import Path
import sys
import unittest

sys.path.insert(0, str(Path(__file__).resolve().parents[2] / 'components'))
from esphome import config_validation as cv
from esphome.core import CORE


class MetadataSchemaTests(unittest.TestCase):
    def schema(self, platform):
        CORE.reset()
        return importlib.import_module('iq2020.' + platform).CONFIG_SCHEMA

    def config(self, platform, value):
        result = {'name': 'Test', 'datapoint': value}
        if platform == 'fan':
            result['speeds'] = 2
        if platform == 'text':
            result['mode'] = 'text'
        return result

    def test_duration_metadata_preserves_values(self):
        fields = {'iq_hoursleft': 'h', 'salt_cartridge_age_days': 'd', 'salt_days_since_manual_test': 'd', 'salt_generation_hours': 'h'}
        config = self.schema('sensor')({key: {'name': key} for key in fields})
        for key, unit in fields.items():
            self.assertEqual(config[key]['unit_of_measurement'], unit)
            self.assertEqual(config[key]['device_class'], 'duration')
            self.assertNotIn('filters', config[key])
        self.assertEqual(config['iq_hoursleft']['state_class'], 'measurement')
        self.assertEqual(config['salt_generation_hours']['state_class'], 'total_increasing')

if __name__ == '__main__':
    unittest.main()
