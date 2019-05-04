#!/usr/bin/env bash

workspace=$(dirname $0)/..
imageRef=station-montage:latest

cd $workspace

docker build -t $imageRef .
