"""Extract current C++ methods; never maintain a second copy of driver logic."""
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
COMPONENT = ROOT / 'components' / 'iq2020'


def block(text, marker):
    start = text.index(marker)
    opening = text.index('{', start)
    depth = 0
    tokens = re.finditer(r'//[^\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[{}]', text[opening:])
    for token in tokens:
        if token.group() == '{':
            depth += 1
        elif token.group() == '}':
            depth -= 1
            if depth == 0:
                return text[start:opening + token.end()]
    raise ValueError(f'Unbalanced source block: {marker}')


def body(text, marker):
    selected = block(text, marker)
    return selected[selected.index('{') + 1:-1]


def generate(kind, component=COMPONENT):
    source = (component / 'iq2020.cpp').read_text()
    template = (Path(__file__).parent / f'{kind}.cpp.in').read_text()
    constants = '\n'.join(line for line in (component / 'iq2020.h').read_text().splitlines() if line.startswith('#define '))
    if kind == 'controls':
        methods = [block(source, 'bool validNumberValue(')]
        methods += [block(source, f'void IQ2020Component::{name}(') for name in
                   ['numberAction', 'setNumberState', 'loop', 'sendIQ2020Command', 'switchAction', 'setSwitchState',
                    'selectAction', 'setSelectState', 'setTempAction', 'sendPendingTemperature', 'reconcileTemperature']]
        methods.append(block((COMPONENT / 'iq2020.h').read_text(), 'void set_active(').replace('void set_active(', 'void IQ2020Component::set_active('))
        methods += [block((COMPONENT / 'number/iq2020_number.cpp').read_text(), 'void IQ2020Number::control('),
                    block((COMPONENT / 'fan/iq2020_fan.cpp').read_text(), 'void IQ2020Fan::control(')]
        ack = block(source, 'if ((pending_temp != NOT_SET) && (cmdlen == 9)')
        methods.append('void IQ2020Component::acknowledgeTemperature() { int cmdlen=9; unsigned char processingBuffer[9]={0,0,0,0,0,1,9,6,0};\n' + ack + '\n}')
    elif kind == 'rx':
        methods = [block(source, signature) for signature in ['int readCounterEx(', 'void IQ2020Component::processRawIQ2020Data(', 'int IQ2020Component::nextPossiblePacket(']]
        prefix = source[source.index('int IQ2020Component::processIQ2020Command() {'):source.index('\n\t// Spa Connection Kit -> IQ2020 Request')]
        iq = block(source, 'if ((processingBuffer[2] == 0x37)')
        methods.append(prefix + '\n' + iq + '\nreturn cmdlen;\n}')
    elif kind == 'audio':
        methods = ['void titleReply(){int responded=0;\n' + body(source, 'else if (processingBuffer[6] == 0x06) { // Song title') + '\n}',
                   'void artistReply(){int responded=0;\n' + body(source, 'else if (processingBuffer[6] == 0x07) { // Artist name') + '\n}']
    elif kind == 'temperature':
        methods = [block(source, 'float parseTemperature(')]
        start = source.index('// Detect units from an intact field')
        end = source.index('#ifdef USE_SENSOR\n\t\t\t// Temperature sensors', start)
        methods += ['void IQ2020Component::temperatures(){float heaterActive=0;\n' + source[start:end] + '\n}']
    elif kind == 'select':
        methods = [block((component / 'select/iq2020_select.cpp').read_text(), 'void IQ2020Select::publish_state_ex(')]
    elif kind == 'outbound':
        methods = [block(source, 'void IQ2020Component::sendIQ2020Command(')]
    elif kind == 'poll':
        methods = [block(source, 'void IQ2020Component::pollState(')]
    else:
        raise ValueError(kind)
    return constants + '\n' + template.replace('// PRODUCTION_METHODS', '\n'.join(methods))
