# For building use this image
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build-image

RUN mkdir -p /build/build_artifacts
WORKDIR /build

COPY ./ /build/
WORKDIR ./Simple.Lambda.DotNet/

# Run build of project
RUN dotnet publish --output /build/build_artifacts --configuration "Release" /p:GenerateRuntimeConfigurationFiles=true --runtime linux-x64 --self-contained false

# For deploying use this image
FROM public.ecr.aws/lambda/dotnet:core3.1 AS publish-image

COPY --from=build-image ./build/build_artifacts /var/task

CMD ["Simple.Lambda.DotNet::Simple.Lambda.DotNet.Function::FunctionHandler"]