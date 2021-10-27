# How to run aws localstack

Example how to run with enabled only lambda functionality:

```
docker run --name localstack-for-lambda -d -p 4566:4566 --platform linux -e SERVICES=lambda localstack/localstack
```

>NOTE: use `--platform linux` only if you run Docker for windows containers with experimental flag set on true that allows run also linux containers.

To remove the container run:
```
docker rm -f localstack-for-lambda
```