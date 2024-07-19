#!/bin/bash

if [[ ! -d certs ]]
then
    mkdir certs
    cd certs/
    if [[ ! -f localhost.pfx ]]
    then
        dotnet dev-certs https -v -ep localhost.pfx -p 1a25a865-cf6d-4252-9df5-dad17144d7ad -t
    fi
    cd ../
fi

docker-compose up -d
