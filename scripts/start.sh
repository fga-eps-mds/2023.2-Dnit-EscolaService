#!/bin/bash

BUILD_NUMBER="$(ls target/ | sort -rn | head -n1)"
WORK_DIR="target/$BUILD_NUMBER/build"

echo "Starting build $BUILD_NUMBER";

cd $WORK_DIR
./app
