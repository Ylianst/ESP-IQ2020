# Development checks

Use Python 3.12 and `pip install -r requirements-test.txt` for the pinned ESPHome baseline.
Host tests need Clang or GCC with AddressSanitizer and UndefinedBehaviorSanitizer.

Run available native tests with `python -m unittest discover -s tests -v`.
Run each `tests/esphome/test_*_schemas.py` file with the ESPHome Python environment.
Generate or compile a selected configuration with
`python tests/esphome/check.py --stage compile --case full`.
The script generates dummy credentials and never uploads firmware.

Each workflow selects only the cases covered by its contribution. The shared
build helper also contains optional-platform scenarios that require the optional
platform fixes; do not interpret their presence as a claim that every revision
supports them. Each contribution's notes in `documentation/` record its actual
results and pending hardware checks.

Native harnesses extract production methods; they stub framework and transport
boundaries. They are not full firmware or real-device simulations. The protocol
fixture, where included, is a public upstream example, not an Aria capture.
Legacy tests cover targeted receive/audio memory backports only.
