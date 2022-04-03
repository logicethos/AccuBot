FROM mcr.microsoft.com/dotnet/sdk:6.0.201-focal-amd64

ARG BRANCH="main"

# Set the timezone.
ENV TZ=UTC
RUN ln -fs /usr/share/zoneinfo/UTC /etc/localtime


RUN apt update \
	&& apt -y install joe less gnupg ssh wget curl net-tools iputils-ping  git locales locales-all mtr-tiny 
    #libncurses5-dev autoconf libncursesw5-dev \
    
#RUN mkdir -p /app
#WORKDIR /app
#RUN git clone --branch v3.5.2 https://github.com/SIPp/sipp.git
#WORKDIR /app/sipp
#RUN ./build.sh

WORKDIR /
RUN git clone -b ${BRANCH} https://github.com/logicethos/AccuBot.git
WORKDIR /AccuBot
RUN dotnet build AccuBotCommon --configuration Release -property:GitCommit=$(git rev-parse HEAD)
RUN dotnet build AccuBot --configuration Release -property:GitCommit=$(git rev-parse HEAD)

    
RUN apt clean
RUN rm -rf /var/lib/apt/lists/*

#COPY entrypoint.sh /entrypoint.sh
#RUN chmod +x /entrypoint.sh

ENTRYPOINT ["dotnet /AccuBot/AccuBot/bin/Release/Accubot.exe"]
