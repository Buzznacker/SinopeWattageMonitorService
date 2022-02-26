FROM mcr.microsoft.com/dotnet/runtime:5.0
COPY bin/Debug/net5.0 SinopeWattageMonitorService/
WORKDIR /SinopeWattageMonitorService

ENV SINOPEEMAIL=youremail@email.com
ENV SINOPEPASSWORD=mycoolpassword

ENTRYPOINT ["dotnet", "SinopeWattageMonitorService.dll"]