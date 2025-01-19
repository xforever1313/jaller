FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Possible options:
# arm64
# x64
ARG CPU_ARCH=x64

# Possible options:
# linux
# win
ARG OS=linux

RUN mkdir -p /app/ && \
    mkdir -p /config/ && \
    mkdir -p /data/ && \
    apt update -y && \
    apt install unzip -y

ADD "https://files.shendrick.net/projects/jaller/releases/latest/Jaller_${OS}-${CPU_ARCH}.zip" /tmp/Jaller.zip

RUN unzip /tmp/Jaller.zip -d /app/ && \
    rm /tmp/Jaller.zip && \
    apt remove wget unzip -y

ADD JallerConfig.cs /config/JallerConfig.cs

ENTRYPOINT [ "dotnet", "/app/bin/Jaller.dll", "--config_file=/config/JallerConfig.cs" ]
