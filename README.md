# TLDR
Simple Ssl checker, inspired by necessity and https://github.com/Matty9191/ssl-cert-check, but faster and (for me) with readable source.

use `RedSslChecker -h` to see avilible options.

By default will try to read `ssldomains` and work with them.

# EXAMPLES
Example: `RedSslChecker -f examplessldomains`
Output:
```
┌─────────────┬────────┬─────────────────────┬──────┐
│ Domain      │ Status │ ExpirationDate      │ Days │
├─────────────┼────────┼─────────────────────┼──────┤
│ example.com │ Valid  │ 2026-01-16 01:59:59 │ 198  │
│ example.com │ Valid  │ 2026-01-16 01:59:59 │ 198  │
└─────────────┴────────┴─────────────────────┴──────┘
```