import boto3
import json
from boto3.dynamodb.conditions import Key
from decimal import Decimal

dynamodb = boto3.resource('dynamodb')
table = dynamodb.Table('vitalStatsVB')

def convert_dynamodb_item(item):
    converted = {}
    for key, value in item.items():
        if isinstance(value, Decimal):
            converted[key] = float(value) if '.' in str(value) else int(value)
        elif isinstance(value, dict) and 'N' in value:
            num_str = value['N']
            converted[key] = float(num_str) if '.' in num_str else int(num_str)
        elif isinstance(value, dict) and 'S' in value:
            converted[key] = value['S']
        else:
            converted[key] = value
    return converted

def lambda_handler(event, context):
    try:
        response = table.scan()
        transformed_items = [convert_dynamodb_item(item) for item in response['Items']]

        # Return the Python object directly - API Gateway will serialize it
        return {
            'statusCode': 200,
            'headers': {'Content-Type': 'application/json'},
            'body': transformed_items  # Remove json.dumps here
        }
    except Exception as e:
        return {
            'statusCode': 500,
            'body': {'error': str(e)}  # Remove json.dumps here
        }
