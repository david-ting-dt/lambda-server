#!/bin/bash

echo Build started on `date`
cp ./src/HelloWorld/bin/Debug/netcoreapp3.1/HelloWorld.deps.json ./
aws cloudformation package --template-file lambdatemplate.yaml --s3-bucket "david-ting-dn-lambda-deployment-artifacts" --output-template-file outputtemplate.yaml