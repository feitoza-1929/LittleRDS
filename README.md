# LittleRDS
A simple key/value database in memory based on Redis

# How does it work?
## Connection
Redis CLI (Client)  could be used to communicate with LittleRDS (Server).
You should connect to LittleRDS with this command `redis-cli -h 127.0.0.1 -p {port}`, after that you can start sending commands to the server.

## Commands
LittleRDS supports only four basic commands without options
| Command    | Description |
| ---------- | ----------- |
| SET        | store data as key/value        |
| GET        | fetch the value related to the given key         |
| HSET       | store data as a set of key/value elements, pretty much like a dictionary       |
| HGET       | fetch the value related to the given key and field        |


## Design
![LittleRDS process flow diagram](./LittleRDSDiagram.png)
>[!NOTE]
>AOF is responsible for the persistence of the data, saving every command (SET and HSET) that changes the database and loads them when the server starts.
> That's just replayed every command until reaches the final database state.

