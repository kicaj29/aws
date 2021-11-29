- [Create first mocked API](#create-first-mocked-api)
- [General API GW features](#general-api-gw-features)
- [Request-response cycle](#request-response-cycle)
- [API with Lambda](#api-with-lambda)
  - [Create API](#create-api)
  - [Create lambda](#create-lambda)
  - [Call the API and solve CORS problem](#call-the-api-and-solve-cors-problem)
  - [Change lambda to use request body](#change-lambda-to-use-request-body)
    - [Integration Request with enabled **Use Lambda Proxy integration**](#integration-request-with-enabled-use-lambda-proxy-integration)


# Create first mocked API

![001_create_api_gw.png](./images/001_create_api_gw.png)

![002_create_api_gw.png](./images/002_create_api_gw.png)

![003_create_api_gw.png](./images/003_create_api_gw.png)

![004_create_api_gw.png](./images/004_create_api_gw.png)

![005_create_api_gw.png](./images/005_create_api_gw.png)

![006_create_api_gw.png](./images/006_create_api_gw.png)

![007_create_api_gw.png](./images/007_create_api_gw.png)

![008_create_api_gw.png](./images/008_create_api_gw.png)

In integration response use mapping section to define the response:

![009_create_api_gw.png](./images/009_create_api_gw.png)

Next we have to deploy the API to a stage - **Resources** section contains only new configuration that is not visible in run time.

![010_create_api_gw.png](./images/010_create_api_gw.png)

![011_create_api_gw.png](./images/011_create_api_gw.png)

Next we can see deployed stage and call the API:

![012_create_api_gw.png](./images/012_create_api_gw.png)

![013_create_api_gw.png](./images/013_create_api_gw.png)

# General API GW features


![014_general_features.png](./images/014_general_features.png)

* API Keys: used when API will be used by developers (or by other backend services - not users???). Then such key has to be sent in every requests.
* Usage Plans: you can enforce a throttling and quota limit on each API key.
* Client Certificates: to ensure HTTP requests to your back-end services are originating from API Gateway, you can use Client Certificates to verify the requester's authenticity.
* Authorizers: Authorizers enable you to control access to your APIs using Amazon Cognito User Pools or a Lambda function.
* Models: using json schema we can validate incoming requests and also use it to map requests and to map responses.

# Request-response cycle

![015_request-response.png](./images/015_request-response.png)

# API with Lambda

## Create API

![016_api_lambda.png](./images/016_api_lambda.png)

![017_api_lambda.png](./images/017_api_lambda.png)

* **Configure as proxy resource**: causes that this resource will catch all possible sub-paths and verbs.

* **Enable CORS**: it will add necessary headers to OPTIONS response:
![018_api_lambda.png](./images/018_api_lambda.png)

and in the view of integration response we can see values of these headers:

![027_CORS.png](./images/027_CORS.png)

Next create a POST endpoint:

![019_api_lambda.png](./images/019_api_lambda.png)

* **Use Lambda Proxy integration**: it will cause that full request with its metadata like headers, authentication data will be send to the lambda as unfiltered. It means that in the lambda function we have to extract what we need. In general it is not recommended approach because it breaks single responsibility pattern. Logic related with API rather should stay in the API GW.

## Create lambda

Next create a lambda function:

![020_api_lambda.png](./images/020_api_lambda.png)

![021_api_lambda.png](./images/021_api_lambda.png)

js
```
exports.handler = (event, context, callback) => {
    // callback is a method that is used to return info from the lambda function
    // first paramter is used to pass errors, here we do not have any errors so we pass null,
    // second param is the resposne
    callback(null, {message: 'I am lambda function'});
};
```

It is import to point which java-script function should be called when the lambda is called: `[file-name].[method-name]`

![022_api_lambda.png](./images/022_api_lambda.png)

Sometimes we want increase default timeout for the lambda function:

![023_api_lambda.png](./images/023_api_lambda.png)

Next we have to publish the lambda function:

![024_api_lambda.png](./images/024_api_lambda.png)

Next we can assign lambda function to the API GW endpoint:

![025_api_lambda.png](./images/025_api_lambda.png)

![026_api_lambda.png](./images/026_api_lambda.png)

Next we can deploy the API to dev stage.

## Call the API and solve CORS problem

```js
var xhr = new XMLHttpRequest();
xhr.open('POST', 'https://3d20ljuw26.execute-api.eu-central-1.amazonaws.com/dev/api-lambda');
xhr.onreadystatechange = function(event) {
  console.log(event.target.response);
}
xhr.send();
```

Next go to https://jsfiddle.net/ to run this script. If we run that we will see that we get CORS error.

It does not work because created POST endpoint also has to return proper headers to enable CORS - it is not enough to do it on OPTIONS verb.

First add the header to **Method Response**:

![028_api_lambda.png](./images/028_api_lambda.png)

and next set its value in the **Integration Response**:

![029_api_lambda.png](./images/029_api_lambda.png)

Now we have to deploy the new version of the API and next we can check if it is working fine:

![030_api_lambda.png](./images/030_api_lambda.png)

## Change lambda to use request body


```js
exports.handler = (event, context, callback) => {
    // callback is a method that is used to return info from the lambda function
    // first paramter is used to pass errors, here we do not have any errors so we pass null,
    // second param is the resposne
    callback(null, event);
};
```

Go to test button and send some payload:

![031_api_lambda.png](./images/031_api_lambda.png)

```json
{
    "name": "Jacek",
    "age": 28
}
```

We can see that response body returns the whole request body.

![032_api_lambda.png](./images/032_api_lambda.png)


### Integration Request with enabled **Use Lambda Proxy integration**

Select option **Use Lambda Proxy integration** it will cause that the whole request will be passed to the lambda function. In such case we do not use any API GW built-in features and we have to take care to return proper CORS headers in the lambda function:

```js
exports.handler = (event, context, callback) => {
    // callback is a method that is used to return info from the lambda function
    // first paramter is used to pass errors, here we do not have any errors so we pass null,
    // second param is the resposne
    console.log('Event content: ' + JSON.stringify(event));
    callback(null, {headers: {'Access-Control-Allow-Origin': '*'}});
};
```

Next we can call the lambda:

![033_api_lambda.png](./images/033_api_lambda.png)

>NOTE: if we would try run lambda version when it returns the whole request as a response then with enabled **Use Lambda Proxy integration** it would not work because request schema does not match to response schema.

In AWS Cloud Watch we can that the **event** parameter contains the whole request with all its field:


![034_api_lambda.png](./images/034_api_lambda.png)

Unusually this approach is not recommended because then lambda has to do some extra logic that normally is responsibility of API GW.
