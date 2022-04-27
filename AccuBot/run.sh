docker build . -t accubot --no-cache
docker run -d \
       -v "accubot:/data" \
       -v "accubot-www:/www" \
       -v "/var/run/docker.sock:/var/run/docker.sock" \
       -e ACCUBOT_WWW=/www \
       -e ACCUBOT_DATA=/data \
       -e ACCUBOT_DOMAIN="red2.logicethos.com" \
       --network host --name accubot accubot
