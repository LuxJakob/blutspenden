import boto3
import json
from boto3.dynamodb.conditions import Key
from datetime import date, datetime
from decimal import Decimal

dynamodb = boto3.resource('dynamodb')
table = dynamodb.Table('vitalStatsVB')

def calculate_totals(items):
    if not items:
        return {}

    blood_counter = 0
    plasma_counter = 0
    max_blood_donations = 6
    current_year = datetime.today().year

    for item in items:
        donation_date = item.get('donation_date', 0)
        date_obj = datetime.strptime(donation_date, "%Y-%m-%dT%H:%M:%S")
        year = date_obj.year
        if year == current_year:
            if item.get('donation_type', 0) == 'blood':
                blood_counter += 1
            if item.get('donation_type', 0) == 'plasma':
                plasma_counter += 1

    max_plasma_donations = 60 - blood_counter
    donation_times = blood_counter + plasma_counter

    totals = {
            'donation_times': donation_times,
            'blood_donations': blood_counter,
            'max_blood_donations': max_blood_donations,
            'plasma_donations': plasma_counter,
            'max_plasma_donations': max_plasma_donations
    }

    return totals

def lambda_handler(event, context):
    try:
        response = table.scan()
        items = response['Items']
        totals = calculate_totals(items)

        return {
            'statusCode': 200,
            'headers': {'Content-Type': 'application/json'},
            'body': totals
        }
    except Exception as e:
        return {
            'statusCode': 500,
            'body': {'error': str(e)}
        }
