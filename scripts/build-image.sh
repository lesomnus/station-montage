#!/usr/bin/env bash

workspace=$(dirname $0)/..
imageRef=lokomotes/station-dotnet:latest

cd $workspace

docker build -t $imageRef .