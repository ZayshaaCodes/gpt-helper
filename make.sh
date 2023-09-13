#!/bin/bash
dotnet publish -c Release -r linux-x64 --self-contained
sudo rm /usr/local/bin/gt
echo 
sudo ln -s $(pwd)/bin/Release/net7.0/linux-x64/publish/gpt-helper /usr/local/bin/gt
