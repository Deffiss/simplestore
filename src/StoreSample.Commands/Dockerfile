FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
ENV ASPNETCORE_URLS http://*:5001
EXPOSE 5001
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "StoreSample.Commands.dll"]
