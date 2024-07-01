# Clone repository from git
FROM alpine/git:latest AS git

ARG GIT_REPOSITORY_TOKEN
ARG GIT_REPOSITORY_PROTOCOL="https"
ARG GIT_REPOSITORY_URL
ARG GIT_REPOSITORY_BRANCH

WORKDIR /repository

RUN git clone "${GIT_REPOSITORY_PROTOCOL}://${GIT_REPOSITORY_TOKEN}@${GIT_REPOSITORY_URL}" .
RUN git checkout "${GIT_REPOSITORY_BRANCH}"

# Build dotnet function
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build_dotnet
ARG NUGET_REPOSITORY_USERNAME
ARG NUGET_REPOSITORY_PASSWORD
ARG NUGET_REPOSITORY_NAME="clouded"
ARG NUGET_REPOSITORY_URL="https://nuget.pkg.github.com/Younics/index.json"
ARG EXECUTABLES_DIR="/executables"

WORKDIR /build
COPY --from=git /repository /build

RUN apt update
RUN apt-get install rename -y
RUN wget -qO /usr/local/bin/yq https://github.com/mikefarah/yq/releases/latest/download/yq_linux_amd64
RUN chmod a+x /usr/local/bin/yq

RUN dotnet nuget add source --username "${NUGET_REPOSITORY_USERNAME}" \
							--password "${NUGET_REPOSITORY_PASSWORD}" \
							--store-password-in-clear-text \
							--name "${NUGET_REPOSITORY_NAME}" "${NUGET_REPOSITORY_URL}"

COPY build_dotnet .
RUN chmod +x build_dotnet
RUN mkdir -p "${EXECUTABLES_DIR}"
RUN ./build_dotnet "${EXECUTABLES_DIR}"

# Run function provider with all prebuilt executables
FROM repository.younics.com/clouded-provider-function/clouded-provider-function-dev:latest
ARG EXECUTABLES_DIR="/executables"

COPY --from=build_dotnet "${EXECUTABLES_DIR}" "${EXECUTABLES_DIR}"