FROM mono:latest

ENV DNX_VERSION 1.0.0-rc1-final
ENV DNX_USER_HOME /opt/dnx
#Currently the CLR packages don't have runtime ids to handle debian:jessie but
#we are making sure that the dependencies are the right versions and are opting for
#the smaller base image. So we use this variable to overwrite the default detection.
ENV DNX_RUNTIME_ID ubuntu.14.04-x64

RUN apt-get -qq update && apt-get -qqy install unzip libc6-dev libicu-dev && rm -rf /var/lib/apt/lists/*

RUN curl -sSL https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.sh | DNX_USER_HOME=$DNX_USER_HOME DNX_BRANCH=v$DNX_VERSION sh
RUN bash -c "source $DNX_USER_HOME/dnvm/dnvm.sh \
	&& dnvm install $DNX_VERSION -alias default \
	&& dnvm alias default | xargs -i ln -s $DNX_USER_HOME/runtimes/{} $DNX_USER_HOME/runtimes/default"

# Install libuv for Kestrel from source code (binary is not in wheezy and one in jessie is still too old)
# Combining this with the uninstall and purge will save us the space of the build tools in the image
RUN LIBUV_VERSION=1.4.2 \
	&& apt-get -qq update \
	&& apt-get -qqy install autoconf automake build-essential libtool git zip \
	&& curl -sSL https://github.com/libuv/libuv/archive/v${LIBUV_VERSION}.tar.gz | tar zxfv - -C /usr/local/src \
	&& cd /usr/local/src/libuv-$LIBUV_VERSION \
	&& sh autogen.sh && ./configure && make && make install \
	&& rm -rf /usr/local/src/libuv-$LIBUV_VERSION \
	&& ldconfig \
	&& apt-get -y purge autoconf automake build-essential libtool \
	&& apt-get -y autoremove \
	&& apt-get -y clean \
	&& rm -rf /var/lib/apt/lists/*
	
Run ["git", "clone", "https://git.oschina.net/mainspace/asp-net.git", "/app/"]

ENV PATH $PATH:$DNX_USER_HOME/runtimes/default/bin

# Prevent `dnu restore` from stalling (gh#63, gh#80)
ENV MONO_THREADS_PER_CPU 50

VOLUME /v
WORKDIR /app/src/CardWarWEB/
RUN ["dnu", "restore"]

EXPOSE 80

ENTRYPOINT ["dnx", "kestrel"]
