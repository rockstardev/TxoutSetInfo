# TxoutSetInfo

This project provides detailed history of Bitcoin UTXO set published by various individuals from Bitcoin ecosystem. Primary use case is quick verification of UTXO set snapshots ([visit FastSync page for more details](https://github.com/btcpayserver/btcpayserver-docker/tree/master/contrib/FastSync)).

See codebase in action on: https://twitter.com/TxoutSetInfo

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

Then realize that it would be very useful if there was a public repository to which various individuals from Bitcoin ecosystem can publish exact output of `gettxoutsetinfo` on every block. This would allow for quick verification of any UTXO snapshot that's published online; so long as information matches and you trust those that came to consensus on `gettxoutsetinfo` output.

Also, this service is valuable for other reasons:

1. Broadcasting state of Bitcoin blockchain that is independent from miners
2. Detecting unintentional splits between different versions of Bitcoin Core

## Forking

I would encourage you to fork codebase and create your own cluster of validators that will broadcast state of UTXO set that group you belong to agrees on. The more people out there we have broadcasting the information on UTXO set specifics, the easier it will be deduce what was consensus at any point of time (block).
