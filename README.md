# RedisSearchProducts

This project shows an example of using Redis and [Set Theory](https://plato.stanford.edu/entries/set-theory/#:~:text=Set%20theory%20is%20the%20mathematical,whose%20members%20are%20also%20sets.) to create a product search page/api. Products are seeded into Redis in a specific way to make look ups and searches very efficient. Included is a fuzzy search and auto-complete/suggestion sample in the search bar. This is using the Redis Search module.

This is not production ready code. Further considerations must be made depending on your Redis configuration using OSS/Enterprise clustering.
