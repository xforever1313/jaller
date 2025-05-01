# Jaller

Jaller is an advanced [IPFS](https://ipfs.tech/) Gateway.  It sits in front of an IPFS node and controls which files are able to be downloaded from the gateway.  It also provides a method to catalog the IPFS CIDs of a file; making IPFS somewhat searchable.

## What is IPFS?

[IPFS](https://en.wikipedia.org/wiki/InterPlanetary_File_System), or InterPlanetary File System, is a file sharing peer-to-peer network for that uses content addressing to retrieve files.  This addressing is called a Content ID (CID).  Each file that is stored on IPFS has its own unique CID, and be retrieved by giving the CID to an IPFS client or from a http IPFS gateway.

## What is an IPFS Gateway?

The recommended approach to downloading a file from IPFS is to use an [IPFS client](https://docs.ipfs.tech/install/ipfs-desktop/).  However, that does require some amount of setup and technical knowledge to setup.  A more convenient way of downloading files from IPFS is to use a gateway; allowing one to download a file from their web browser.

Gateways, though convenient, sort of go against the intent of IPFS.  It centralizes something that is supposed to be decentralized.  IPFS maintains a list of public Gateways [here](https://ipfs.github.io/public-gateway-checker/), and there are only a handful of them that are active.

If there are more gateways online, the better and more robust the IPFS network will be.  However, [some folks](https://discuss.ipfs.tech/t/public-gateway-operator-guide/662) have legal concerns over running a public IPFS gateway.  While there have been [legal cases](https://torrentfreak.com/ifps-gateway-operator-is-not-liable-for-pirated-software-keys-240223/) where IPFS gateway operators are not liable for how users use their public gateways, it still can be very inconvenient if a company wishes to pursue legal action against a gateway operator.

One of Jaller's use cases is to only allow *approved* files to be downloaded from a gateway operator's gateway.  These approved files the gateway operator should be allowed to distribute.

## CID Cataloguing

One criticism of IPFS is there doesn't appear to be an easy way of searching the network.  You may want a file, but am unsure of what the CID is.  There are various online forums that try to catalog IPFS hashes such as [/r/IPFS_Hashes](https://www.reddit.com/r/IPFS_Hashes/comments/1dpzdab/clear_web_ipfs_database/), but searching those can be daunting.

Jaller provides a method to catalog and search for CIDs.  A Jaller instance can import file metadata from another Jaller instance, and can export its own metadata that other Jaller instances can consume.  It can also be operated in a way where a Jaller operator doesn't want to allow any direct file downloads, but can run the cataloging system.

## Running your own node

### Determine Scope

First, you need to decide what the scope of your Jaller node will be.  Do you want users to download files directly from your instance?  Do you only want to catalog files?  Do you only want to allow files to be downloaded with certain people (e.g. friends and family)?  Once you figure this out, you can configured your Jaller node correctly.

### Installing Jaller

Jaller is under active development.  There is no public release at the moment.
You can compile from source and run it that way, but bare in mind that this is currently pre-alpha quality software.

## Name Meaning

Jaller's name is inspired by a Lego Bionicle character, [Jaller](https://bionicle.fandom.com/wiki/Jaller).  Jaller was the guard captain of his in-universe village, Ta-Koro.  This software aims to be a guard for a user's IPFS gateway, deciding which files can be downloaded by it.

This software is not associated with nor endorsed by Lego.  Just created by one of their fans.
