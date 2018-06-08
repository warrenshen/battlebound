# dev instructions
1. Modify index.js to update the handler

2. Whenever a new package is added/modified, run `npm run install-docker` to get the right package. Note you need to build the image using `docker build -t lambda-node8 .` for the first time.

3. Before uploading to file to aws, run `docker run -it --rm -v "$PWD":/var/task lambci/lambda:nodejs8.10` locally to test the results (you may need to configure your event.

4. Then, zip the files (do not use the mac default one) `zip -r ../lambda.zip *`

5. Lastly, upload the `.zip` file to the aws and test it there.

