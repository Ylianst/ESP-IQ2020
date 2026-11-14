import unittest
from host.support import NativeHarness

class ControlsTests(NativeHarness):
    kinds = ['controls', 'select', 'poll']

    def test_output_packet_and_checksum(self):
        wire = self.results['normal']['wire']
        self.assertEqual(wire[1:5], [0x29, 1, 15, 0x40])
        self.assertEqual(wire[5:8], [0x1e, 1, 4])
        self.assertEqual(wire[-1], (sum(wire[1:-1]) & 255) ^ 255)

    def test_U03_invalid_number_id(self):
        self.assertEqual(self.crashes['invalid_id'], 0)

    def test_U03_fourth_jet_never_targets_salt_module(self):
        self.assertEqual(self.results['fan_alias']['packets'], 0)

    def test_U04_three_retries_are_attempted(self):
        self.assertEqual(self.results['retry']['packets'], 4)

    def test_U05_unchanged_report_corrects_display(self):
        self.assertEqual(self.results['pending']['shown'], 3)

    def test_U05_inactive_command_is_not_displayed_as_confirmed(self):
        self.assertEqual(self.results['inactive']['shown'], 3)

    def test_U06_zero_sends_a_command(self):
        self.assertEqual(self.results['zero']['packets'], 1)

    def test_U07_newer_temperature_survives_old_ack(self):
        self.assertEqual(self.results['temperature']['pending_temp'], 102)

    def test_U08_speed_only_preserves_power(self):
        self.assertEqual(self.results['fan_speed']['fan_on'], 1)

    def test_U08_on_only_preserves_high_speed(self):
        self.assertEqual(self.results['fan_on']['wire'][7], 3)

    def test_retry_expiry_and_ack(self):
        expired=self.success('controls','retry_expiry')
        self.assertEqual((expired['packets'],expired['number_pending'],expired['shown']),(4,-127,3))
        ack=self.success('controls','ack')
        self.assertEqual((ack['packets'],ack['number_pending'],ack['shown']),(1,-127,4))

    def test_independent_controls_keep_their_retry_budgets(self):
        self.assertEqual(self.success('controls','independent')['packets'],12)

    def test_readback_preserves_newer_number(self):
        result=self.success('controls','newer_number')
        self.assertEqual((result['number_pending'],result['shown'],result['wire'][7]),(6,4,6))

    def test_unlabelled_ack_does_not_claim_latest_target(self):
        result=self.success('controls','unlabelled_ack')
        self.assertEqual((result['number_pending'],result['shown']),(6,3))

    def test_inactive_cancels_queued_commands(self):
        result=self.success('controls','cancel_inactive')
        self.assertEqual((result['packets'],result['number_pending']),(1,-127))

    def test_rejected_values_do_not_mutate_intent(self):
        result=self.success('controls','rejected_number')
        self.assertEqual((result['packets'],result['shown'],result['number_pending']),(0,3,-127))
        self.assertEqual(self.success('controls','invalid_setters')['packets'],0)
        self.assertEqual(self.success('controls','invalid_reports')['packets'],2)

    def test_legacy_zero_uses_legacy_route(self):
        result=self.success('controls','legacy_zero')
        self.assertEqual(result['wire'][1:5],[1,31,5,64])
        self.assertEqual(result['wire'][5:-1],[30,2,1,0,0])

    def test_single_speed_and_off_speed_only(self):
        self.assertEqual(self.success('controls','fan_single')['wire'][7],3)
        result=self.success('controls','fan_off_speed')
        self.assertEqual((result['fan_on'],result['wire'][7]),(0,1))
        self.assertEqual(self.success('controls','fan_invalid')['packets'],0)

    def test_temperature_readback_releases_latest_target(self):
        result=self.success('controls','temperature_followup')
        self.assertEqual((result['packets'],result['pending_temp'],result['target'],result['inflight']),(2,102,101,102))
        self.assertEqual(result['wire'][5:-1],[1,9,255,1])
        confirmed=self.success('controls','temperature_confirm')
        self.assertEqual((confirmed['pending_temp'],confirmed['inflight'],confirmed['target']),(-127,-127,102))

    def test_temperature_expiry_and_bounded_retry(self):
        expired=self.success('controls','temperature_expiry')
        self.assertEqual((expired['packets'],expired['pending_temp'],expired['inflight'],expired['target']),(1,-127,-127,100))
        retry=self.success('controls','temperature_retry')
        self.assertEqual((retry['packets'],retry['pending_temp']),(3,-127))
        self.assertEqual(self.success('controls','temperature_rejected')['packets'],0)

    def test_temperature_units_and_negative_delta(self):
        self.assertEqual(self.success('controls','temperature_negative')['wire'][8],255)
        self.assertEqual(self.success('controls','temperature_celsius')['wire'][8],1)

    def test_relative_light_retries_are_bounded(self):
        for case,key in [('light_budget','light_pending'),('color_budget','color_pending')]:
            result=self.success('controls',case)
            self.assertEqual((result['packets'],result[key]),(4,-127))
        result=self.success('controls','light_no_reply')
        self.assertEqual((result['packets'],result['light_pending']),(1,-127))

    def test_select_feedback_bounds(self):
        for datapoint,options,value in [('1','7','0'),('1','8','-1'),('5','4','-1'),('5','4','4'),('999','8','1')]:
            self.assertEqual(self.success('select',datapoint,options,value),{'published':0,'shown':''})
        self.assertEqual(self.success('select','1','8','0'),{'published':1,'shown':'7'})
        self.assertEqual(self.success('select','0','4','4'),{'published':1,'shown':'3'})
        self.assertEqual(self.success('select','5','4','0'),{'published':1,'shown':'0'})

    def test_startup_delay_preserves_discovery_and_version_fallback(self):
        self.assertEqual(self.success('poll','delayed'),{'packets':1,'version_attempts':0,'iq_attempts':1,'last_command':0x23D1})
        self.assertEqual(self.success('poll','fallback'),{'packets':11,'version_attempts':10,'iq_attempts':1,'last_command':0x23D1})

if __name__ == '__main__':
    unittest.main()
