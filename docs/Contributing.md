# Contributing

## General

The project by design is supposed to be minimalistic, so if you have tons of ideas for new functionalities, it is quite possible that creating a fork will be the most reasonable option. 

However, there are some things that it would be nice to have:
- `Redis` support (caching, locking etc.) for easy scale application vertically
- Full translation system. Currently, many elements of the system on the user space still have hardcoded english versions (e.g. information tab)
- Full support for the update event (currently exists, but is not handled)
- Correctly written tests in `xUnit`. Current tests were written in `MSTest` and therefore are not available in the repository