import boto3
import json
from boto3.dynamodb.conditions import Key
from decimal import Decimal

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

    thresholds = []
    for item in items:
        thresholds.append({
            'donation_date': item.get('donation_date', 0),
            'pulse_max': 100,
            'pulse_min': 60,
            'temperature_max': 37.5,
            'temperature_min': 36.1,
            'blood_pressure_lower_max': 80,
            'blood_pressure_lower_min': 60,
            'blood_pressure_upper_max': 120,
            'blood_pressure_upper_min': 90,
            'hemoglobin_max': 17.5,
            'hemoglobin_min': 13.5
        })

    return thresholds