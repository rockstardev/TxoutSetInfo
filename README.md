# TxoutSetInfo

This project tries to provide detailed history of Bitcoin UTXO set, along with detection of unintentional splits between different versions of Bitcoin Core. See codebase in action on: https://twitter.com/TxoutSetInfo

## How to participate

1. Ensure that you have .NET Core 2.2 installed (https://dotnet.microsoft.com/download/dotnet-core/2.2)
2. Download latest release of Fetcher: https://github.com/rockstardev/TxoutSetInfo/releases
3. Fill in config with info https://twitter.com/r0ckstardev provided you with
4. Try if Fetcher runs by executing: `dotnet TxoutSet.Fetcher.dll`
5. Check that your node is now part of consensus on: https://twitter.com/TxoutSetInfo
6. If everything looks good, schedule Fetcher to run every 60 seconds on your machine (use Task Scheduler on Windows or cron/whatever you prefer on Linux)

## Filling out config

Default config coming with release will look something like this:

	{
	  "PublisherUrl": "PATH_TO_PUBLISHER",
	  "PublisherApiKey": "API_KEY_ROCKSTARDEV_GAVE_YOU",
	  "ReadlineAtExit": false,

	  "BitcoindUri": "http://127.0.0.1:8332",
	  "BitcoindCred": "cookiefile=c:\\Users\\YOUR_USERNAME\\AppData\\Roaming\\Bitcoin\\.cookie"
	}

`PublisherUrl` and `PublisherApiKey` values will be provided to you by @rockstardev. 
`ReadlineAtExit` allows for Windows kids to pause closing of terminal window after execution. Just run dedicated terminal window and this variable will be useless for you
`BitcoindUri` and `BitcoinCred` describe how Fetcher will be connecting to your Bitcoind. In most cases you jut need to change path to cookiefile. If you are still using `rpcuser` and `rpcpassword` do: `"BitcoindCred": "rpcuser:rpcpassword"`

## Motivation

Read what @NicolasDorier wrote in FastSync readme: https://github.com/btcpayserver/btcpayserver-docker/tree/master/contrib/FastSync

Then realize that it would be very useful if there was public repository of information on what TxoutSetInfo was during every Bitcoin block. Unfortunately, running `gettxoutsetinfo` command is very expensive, so I've created that's where this dedicated Fetcher comes in.

Also, this service is valuable for two additional reasons:

1. Monitoring UTXO set of different Bitcoin Core versions 
2. Broadcasting state of Bitcoin blockchain that is independent from miners

## Forking

I would encourage you to fork codebase and create your own cluster of validators that will broadcast state of UTXO set that group you belong to agrees on. The more people out there we have broadcasting the information on UTXO set specifics, the easier it will be deduce what was consensus at any point of time (block).