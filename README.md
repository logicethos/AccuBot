## AccuBot
### Accumulate Monitoring Bot

#### To start:

    docker run -d -v "accubot:/data" -v "accubot-www:/www" -v "/var/run/docker.sock:/var/run/docker.sock" -e ACCUBOT_WWW=/www -e ACCUBOT_DATA=/data --network host accubot4

## AccuBotCommon
grpc client DLL
Protcol Buffer messages
