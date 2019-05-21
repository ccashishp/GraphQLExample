# GraphQLExample
Sample GraphQL implementation in transcript

# EndPoints
http://localhost:9140/transcriptgraphql
<pre>
Ex Query
{
	query: "{
	  hello,
	  
	  transcriptrequests{
	  	id,
	  	inunId,
	  	latestHistory{
	  		statusDate,
	  		isCreatedByStudent,
	  		status,
	  		transcriptRequestType
	  	}
	  }
	  
	}"
}

RESULT

{
    "data": {
        "hello": "Congratulations! you got admission.",
        "transcriptrequests": [
            {
                "id": 1041453,
                "inunId": "7358",
                "latestHistory": {
                    "statusDate": "2019-05-18",
                    "isCreatedByStudent": true,
                    "status": "REQUESTED",
                    "transcriptRequestType": "IN_NETWORK"
                }
            }
        ]
    }
}
</pre>


# EndPoints
http://localhost:9140/graphql

<pre>
Ex Query
{
operationName: null,
query: "{
  orders {
    id
    name
    description
  }
}",
variables: null
}

Result:

{
    "data": {
        "orders": [
            {
                "id": "80ed0f78-8f0c-480f-96fc-875a85e5c37a",
                "name": "Nike-Cortez",
                "description": "Blue Nike-Cortez size 45"
            },
            {
                "id": "299bf63e-5547-42b2-aefc-ca6e24af8bd9",
                "name": "Adidas-Teeshort",
                "description": "Grey Adidas-Teeshort size XL"
            }
        ]
    }
}
</pre>
