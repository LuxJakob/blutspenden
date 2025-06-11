import json
import boto3
from decimal import Decimal
from boto3.dynamodb.conditions import Key

dynamodb = boto3.resource('dynamodb')
table = dynamodb.Table('vitalStatsVB')


def lambda_handler(event, context):
    try:
        response = table.scan()
        items = response['Items']
        thresholds = get_thresholds(items)

        return {
            'statusCode': 200,
            'headers': {'Content-Type': 'application/json'},
            'body': thresholds
        }
    except Exception as e:
        return {
            'statusCode': 500,
            'body': {'error': str(e)}
        }


def get_thresholds(items):
    if not items:
        return {}

    donation_dates = []
    for item in items:
        donation_dates.append(item.get('donation_date', 0))

    averages = []
    for date in donation_dates:
        averages.append({
            'donation_date': date
        })


    return 'test'