#!/bin/sh
sudo apt install dotnet-runtime-10.0 install dotnet-sdk-10.0
sudo cp homeassistant-status.service /etc/systemd/system/
sudo systemctl enable `cat service-name.txt`
sudo systemctl start `cat service-name.txt`
