#!/bin/bash

echo "::set-output name=new_release_version::$1"
sed -i "s#<AssemblyVersion>.*#<AssemblyVersion>$1</AssemblyVersion>#" $2
sed -i "s#<Version>.*#<Version>$1</Version>#" $2