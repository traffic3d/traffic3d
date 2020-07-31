FROM gableroux/unity3d:2018.3.11f1

RUN apt-get update -qq && \
    apt-get install --no-install-recommends -y autoconf moreutils && \
    wget https://github.com/ltratt/multitime/archive/multitime-1.4.tar.gz && \
    tar -xvzf multitime-1.4.tar.gz && \
    rm multitime-1.4.tar.gz && \
    cd multitime-multitime-1.4 && \
    make -f Makefile.bootstrap && \
    ./configure && \
    make && \
    make install && \
    cd .. && rm -Rf multitime-multitime-1.4 && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*
