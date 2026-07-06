FROM gcc:14 AS build
WORKDIR /src
COPY . .
RUN make test
RUN make all
