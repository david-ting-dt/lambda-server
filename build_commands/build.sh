#!/bin/bash

echo Restore started on `date`
pushd src/HelloWorld
dotnet clean
dotnet restore
popd