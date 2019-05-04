#!/bin/bash

dir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
out=$(dirname $dir)"/protos"

protos_path="https://raw.github.com/lokomotes/metro/master/api/proto/"
portos=( "common.proto" "router.proto" )

for i in "${portos[@]}"
do
	curl -L $protos_path$i -o $out"/"$i
done
