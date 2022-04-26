FROM mcr.microsoft.com/dotnet/sdk:6.0.201-focal-amd64 AS build

ARG BRANCH="main"

# Set the timezone.
ENV TZ=UTC
RUN ln -fs /usr/share/zoneinfo/UTC /etc/localtime


RUN apt update \
	&& apt -y install joe less gnupg ssh wget curl net-tools iputils-ping  git locales locales-all mtr-tiny bash
    #libncurses5-dev autoconf libncursesw5-dev \

RUN apt clean
RUN rm -rf /var/lib/apt/lists/*

    
#RUN mkdir -p /app
#WORKDIR /app
#RUN git clone --branch v3.5.2 https://github.com/SIPp/sipp.git
#WORKDIR /app/sipp
#RUN ./build.sh

WORKDIR /
RUN git clone -b ${BRANCH} https://github.com/logicethos/AccuBot.git
WORKDIR /AccuBot
RUN dotnet build AccuBotCommon --configuration Release --os linux -property:GitCommit=$(git rev-parse HEAD) --output /app
RUN dotnet build AccuBot --configuration Release --os linux -property:GitCommit=$(git rev-parse HEAD) --output /app
    

#COPY entrypoint.sh /entrypoint.sh
#RUN chmod +x /entrypoint.sh

ENV ACCUBOT_DATA = "/data"
ENV ACCUBOT_WWW = "/www"

WORKDIR /app
ENTRYPOINT ["/usr/bin/dotnet","AccuBot.dll"]
#ENTRYPOINT ["dotnet AccuBot.dll"]
