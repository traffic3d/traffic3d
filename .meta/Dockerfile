FROM gableroux/unity3d:2018.3.11f1

RUN apt-get update -qq && \
    apt-get install -y moreutils && \
    wget https://tratt.net/laurie/src/multitime/releases/multitime-1.4.tar.gz && \
    tar -xvzf multitime-1.4.tar.gz && \
    cd multitime-1.4 && \
    ./configure && make && \
    make install
