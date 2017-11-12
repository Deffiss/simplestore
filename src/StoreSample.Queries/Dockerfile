FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
ENV ASPNETCORE_URLS http://*:5002
EXPOSE 5002
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "StoreSample.Queries.dll"]
