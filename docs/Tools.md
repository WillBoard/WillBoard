# Tools

## WillBoard.Tools.ImportCountryIp

Application for importing IP number to country mappings from the CSV file into the application database.  CSV schema:

- `IpNumberFrom` should be number
- `IpNumberTo` should be number
- `CountryCode` should be [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2 "ISO 3166-1 alpha-2")

CSV files should be placed in the `Assets` folder (`country-ipv4.csv` and `country-ipv6.csv`). Example files can be found in [ip-location-db](https://github.com/sapics/ip-location-db "ip-location-db") repository.
