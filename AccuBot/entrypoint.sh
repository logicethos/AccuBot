#!/bin/bash

APP_NAME="Accubot.exe"                                                                                                                                                                                                                                                         
APP_DIR="/app/Accubot/Accubot/bin/Release"
APP_DIR_DEBUG="/app/Accubot/Accubot/bin/Debug" 
BUILD_DIR="/app/Accubot"
BUILD_CONF="Release"


function build
{
    cd $BUILD_DIR
    git pull
    dotnet build AccuBotCommon --configuration Release -property:GitCommit=$(git rev-parse HEAD)
    dotnet build AccuBot --configuration Release -property:GitCommit=$(git rev-parse HEAD)
}


exitcode=-1
until [ $exitcode -eq 0 ]
do
        startdate="$(date +%s)"
        cd $APP_DIR
        dotnet $APP_NAME
        exitcode=$?
        enddate="$(date +%s)"
        
        echo "EXIT CODE = $exitcode"
        
        elapsed_seconds="$(expr $enddate - $startdate)"
        echo "Elapsed seconds $elapsed_seconds"
        
        if [ $exitcode -eq 2 ] #Restart
        then
          echo "RESTART"
        elif [ $exitcode -eq 4 ] #Previous version
        then
          echo "PREVIOUS VERSION"
          cp -fv $APP_NAME_previous $APP_NAME
        elif [ $exitcode -eq 3 ] #Update
        then
          echo "SOFTWARE UPDATE"
          BUILD_CONF="Release"
          cp -fv $APP_NAME $APP_NAME_previous
          build
        elif [ $exitcode -eq 0 ] #Shutdown
        then
          echo "SHUTDOWN"
        fi
        
        if [ $elapsed_seconds -lt 30 ]  #been running for less than 30 seconds
        then
                sleep 10  # delay to protect against eating the CPU resourses with infinate loop
        fi

done
echo "BASH: terminate $exitcode"
