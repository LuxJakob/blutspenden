import os
import csv
import json

md_filepath="README.md"
csv_filename='blood_donations.csv'

with open(md_filepath, 'r', encoding='utf-8') as f:
	lines = f.readlines()
modified_content = lines[5:-1]

content_str = ''.join(modified_content)
start_idx = content_str.find('[')
end_idx = content_str.rfind(']') + 1

if start_idx == -1 or end_idx == -1:
	print("Could not find JSON data in the markdown file.")

json_str = content_str[start_idx:end_idx]

try:
	health_data = json.loads(json_str)

	if health_data:
		with open(csv_filename, 'w', newline='', encoding='utf-8') as csvfile:
			fieldnames = health_data[0].keys()
			writer = csv.DictWriter(csvfile, fieldnames=fieldnames)

			writer.writeheader()
			for entry in health_data:
				writer.writerow(entry)

		print(f"Successfully wrote data to {csv_filename}")
	else:
		print("No health data found to write to CSV.")

except json.JSONDecodeError as e:
	print(f"Error parsing JSON data: {e}")
