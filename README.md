# OBS Tally Light

OBS Tally Light provides a scaleable web-based Tally Light system for video mixers. It acts as a broker between different tally sources and destinations, with a focus on removing the need for custom hardware solutions, and it works over the internet.

It can receive tally information from the following sources:

- Open Broadcast Software
- Black Magic ATEM Hardware Switcher (Ethernet based)

It can relay this tally information to the following destinations:

- SPA (Single page static web application) hosted via Firebase (access via web)
- WiFi lamps based on Particle Photon boards (local network only)

OBS Tally Light is a small piece of software that you can run on the same network as your video switcher which acts as the broker.

## Getting Started

Run the OBSBridgeCore console application on a machine that is on the same network as your Switcher.

*For OBS, you need to enable the Websocket Plugin.*

You can use the centrally hosted SPA at https://obstally.web.app by using the default options on the broker. It will ask you to login using a Google account (to make sure that other people cannot write control your tally indicators).

If you want to run only on the local network (i.e. using WiFi lamps) you can use `--offline`.

## Build

dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true