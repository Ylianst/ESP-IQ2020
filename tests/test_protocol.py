import unittest
from host.support import NativeHarness

class ProtocolTests(NativeHarness):
    kinds = ['rx', 'audio', 'temperature', 'outbound', 'legacy_rx', 'legacy_audio']

    def test_documented_packet(self):
        self.assertEqual(self.success('rx', 'normal', self.packet), self.expected)

    def test_fragmented_packet(self):
        self.assertEqual(self.success('rx', 'split', self.packet), self.expected)

    def test_invalid_checksum(self):
        self.assertEqual(self.success('rx', 'bad_checksum', self.packet), {'published': 0})

    def test_twenty_byte_audio_fields(self):
        for field in ['title', 'artist']:
            self.assertEqual(self.success('audio', field, '20'), {'bytes': 22})

    def test_U01_receive_compaction(self):
        self.assertEqual(self.crashes['overlap'], 0)

    def test_U02_title_capacity(self):
        self.assertEqual(self.crashes['title'], 0)

    def test_U02_artist_capacity(self):
        self.assertEqual(self.crashes['artist'], 0)

    def test_receive_compaction_preserves_all_frames(self):
        result=self.success('rx','overlap',self.packet)
        self.assertEqual(result,{**self.expected,'published':3})

    def test_receive_resynchronizes(self):
        self.assertEqual(self.success('rx','resync',self.packet),self.expected)

    def test_audio_empty_oversized_and_multibyte(self):
        for field in ['title','artist']:
            for size,expected in [('0',2),('21',22),('500',22),('utf8',22)]:
                self.assertEqual(self.success('audio',field,size),{'bytes':expected})

    def test_outbound_capacity_includes_checksum(self):
        result=self.success('outbound')
        self.assertEqual(result['packets'],1)
        self.assertEqual(len(result['wire']),64)

    def test_temperature_fields_are_bounded_and_invalid_is_unknown(self):
        for unit,field,value in [('c',' 9.5',9.5),('c','38.0',38),('c',' 0.0',0),('f',' 99F',99),('f','104F',104)]:
            self.assertEqual(self.success('temperature',unit,field),{'valid':True,'value':value})
        for unit,field in [('c','xx.x'),('c','NaN '),('c','  .5'),('c','3x.5'),('f','errF'),('f','99xF'),('f',' 99C')]:
            self.assertEqual(self.success('temperature',unit,field),{'valid':False})

    def test_all_temperature_fields_use_the_bounded_parser(self):
        self.assertEqual(self.success('temperature','packet',' 9.538.010.5'),{'target':9.5,'current':38,'outlet':10.5})
        self.assertEqual(self.success('temperature','packet',' 99F104F103F'),{'target':99,'current':104,'outlet':103})
        for field in range(3):
            values=['38.0','37.5','39.0'];values[field]='xx.x'
            expected={'target':38,'current':37.5,'outlet':39}
            expected[['target','current','outlet'][field]]=None
            self.assertEqual(self.success('temperature','packet',''.join(values)),expected)

    def test_legacy_memory_backports(self):
        self.assertEqual(self.success('legacy_rx','overlap',self.packet),{**self.expected,'published':3})
        for field in ['title','artist']:
            self.assertEqual(self.success('legacy_audio',field,'500'),{'bytes':22})

if __name__ == '__main__':
    unittest.main()
