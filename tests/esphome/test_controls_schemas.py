"""Validate actual external-component schemas with the pinned ESPHome install."""
import importlib
from pathlib import Path
import sys
import unittest

sys.path.insert(0, str(Path(__file__).resolve().parents[2] / 'components'))
from esphome import config_validation as cv
from esphome.core import CORE


class ControlsSchemaTests(unittest.TestCase):
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

    def test_supported_datapoint_boundaries(self):
        for platform, maximum in [('number', 10), ('switch', 10), ('select', 5), ('fan', 2), ('text', 1), ('button', 2)]:
            for value in [0, maximum]:
                with self.subTest(platform=platform, value=value):
                    self.schema(platform)(self.config(platform, value))

    def test_invalid_datapoints_fail_at_the_field(self):
        for platform, maximum in [('number', 10), ('switch', 10), ('select', 5), ('fan', 2), ('text', 1), ('button', 2)]:
            for value in [-1, maximum + 1, 999]:
                with self.subTest(platform=platform, value=value):
                    with self.assertRaisesRegex(cv.Invalid, 'datapoint'):
                        self.schema(platform)(self.config(platform, value))

    def test_fan_speed_counts(self):
        for value in [0, 3, 999]:
            with self.assertRaisesRegex(cv.Invalid, 'speeds'):
                self.schema('fan')({'name': 'Test', 'datapoint': 0, 'speeds': value})
        for value in [1, 2]:
            self.schema('fan')({'name': 'Test', 'datapoint': 0, 'speeds': value})

    def test_text_sensor_can_omit_version(self):
        config = self.schema('text_sensor')({'rtc_datetime': {'name': 'Clock'}})
        self.assertNotIn('versionstr', config)

if __name__ == '__main__':
    unittest.main()
