import json
import boto3
from decimal import Decimal
from boto3.dynamodb.conditions import Key

dynamodb = boto3.resource('dynamodb')
table = dynamodb.Table('vitalStatsVB')

def calculate_averages(items):
    if not items:
        return {}

    totals = {
        'weight_kg': 0,
        'amount_donated_ml': 0,
        'pulse': 0,
        'temperature': 0,
        'blood_pressure_lower': 0,
        'blood_pressure_upper': 0,
        'hemoglobin': 0,
        'count': len(items)
    }

    for item in items:
        totals['weight_kg'] += item.get('weight_kg', 0)
        totals['amount_donated_ml'] += item.get('amount_donated_ml', 0)
        totals['pulse'] += item.get('pulse', 0)
        totals['temperature'] += item.get('temperature', 0)
        totals['blood_pressure_lower'] += item.get('blood_pressure_lower', 0)
        totals['blood_pressure_upper'] += item.get('blood_pressure_upper', 0)
        totals['hemoglobin'] += item.get('hemoglobin', 0)

    averages = {
        'average_weight_kg': round(totals['weight_kg'] / totals['count'], 2),
        'average_amount_donated_ml': round(totals['amount_donated_ml'] / totals['count']),
        'average_pulse': round(totals['pulse'] / totals['count']),
        'average_temperature': round(totals['temperature'] / totals['count'], 2),
        'average_blood_pressure_lower': round(totals['blood_pressure_lower'] / totals['count']),
        'average_blood_pressure_upper': round(totals['blood_pressure_upper'] / totals['count']),
        'average_hemoglobin': round(totals['hemoglobin'] / totals['count'], 2),
        'total_donations': totals['count']
    }

    return averages

def lambda_handler(event, context):
    try:
        response = table.scan()
        items = response['Items']
        averages = calculate_averages(items)

        return {
            'statusCode': 200,
            'headers': {'Content-Type': 'application/json'},
            'body': averages
        }
    except Exception as e:
        return {
            'statusCode': 500,
            'body': {'error': str(e)}
        }
