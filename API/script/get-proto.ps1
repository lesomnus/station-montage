$PROJECT_PATH =  Join-Path $PSScriptRoot ".." -Resolve
$OUT = Join-Path $PROJECT_PATH "protos"

$PROTOS_PATH = "https://raw.github.com/lokomotes/metro/master/api/proto/"
$PROTOS = "common.proto", "router.proto"

foreach($proto in $PROTOS){
    Invoke-WebRequest -Uri $PROTOS_PATH$proto -OutFile $(Join-Path $OUT $proto)
}
