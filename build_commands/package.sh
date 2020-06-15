#!/bin/bash

echo Build started on `date`
aws cloudformation package --template-file lambdatemplate.yaml --s3-bucket "david-ting-dn-lambda-deployment-artifacts" --output-template-file outputtemplate.yaml