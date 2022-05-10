# BinaryMessage - Simple Binary Message Encoding Schema implementation

## Assumptions: 
- not error checks with CRC or hash used
- there is a check for start sequence during decoding - this can be safely removed to save 3 bytes if needed
- If input message contains non supported data, exception is thrown and encoding aborted 
- At least one header should be always present 
- Payload should be always present

## Optimizations:
- Size of key/value can be shorted
