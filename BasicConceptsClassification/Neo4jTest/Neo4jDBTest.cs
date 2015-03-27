using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Neo4j;
using BCCLib;
using System.Collections.Generic;
using System.Linq;

using System.Diagnostics;

namespace Neo4jTest
{
    [TestClass]
    public class Neo4jDBTest
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var conn = new Neo4jDB();

            conn.cleanupTestMess();
        }

        [TestMethod]
        public void Neo4jDB_OpenConnection_Successful()
        {
            var TestConnection = new Neo4jDB();
            TestConnection.open();
            Assert.IsNotNull(TestConnection);
        }

        [TestMethod]
        public void AddClassiier_Successful()
        {
            GLAM glam = new GLAM("US National Parks Service");

            Classifier classifier = new Classifier(glam);
            classifier.email = "user99@USNationalParks.com";
            
            var conn = new Neo4jDB();

            Classifier addedClassifier = conn.addClassifier(classifier);

            Assert.IsNotNull(addedClassifier);
            Assert.AreEqual(classifier.email, addedClassifier.email);
            Assert.AreEqual(glam.name, addedClassifier.getOrganizationName());

            conn.deleteClassifier(addedClassifier);
        }

        [TestMethod]
        public void AddClassifier_AlreadyExists_ThrowException()
        {
            GLAM glam = new GLAM("US National Parks Service");

            Classifier classifier = new Classifier(glam);
            classifier.email = "user99@USNationalParks.com";

            var conn = new Neo4jDB();

            Classifier addedClassifier = conn.addClassifier(classifier);
           
            // TODO: once we have test setup and cleanup, use those instead
            // of try-catch to do the clean up...
            try
            {
                Classifier addedClassifier2 = conn.addClassifier(addedClassifier);
            }
            catch (Exception)
            {
            }
            conn.deleteClassifier(addedClassifier);
            
        }

        [TestMethod]
        public void GetClassifier_ByEmail_Exists()
        {
            string classifierEmail = "user1@USNationalParks.com";

            var conn = new Neo4jDB();

            Classifier foundClassifier = conn.getClassifier(classifierEmail);

            Assert.IsNotNull(foundClassifier);
            Assert.AreEqual(classifierEmail, foundClassifier.email);
             
        }

        [TestMethod]
        public void GetClassifier_ByEmail_DoesNotExist()
        {
            string classifierEmail = "userDoesNotExist@USNationalParks.com";

            var conn = new Neo4jDB();

            Classifier foundClassifier = conn.getClassifier(classifierEmail);

            Assert.IsNull(foundClassifier);
        }

        [TestMethod]
        public void DeleteClassifier_Successful()
        {
            GLAM glam = new GLAM("US National Parks Service");
            Classifier classifier = new Classifier(glam);
            classifier.email = "userDeleteMe@USNationalParks.com";

            var conn = new Neo4jDB();

            Classifier addedClassifier = conn.addClassifier(classifier);

            conn.deleteClassifier(addedClassifier);

            // Make sure the classifier no longer exists
            Classifier notFound = conn.getClassifier(addedClassifier.email);
            Assert.IsNull(notFound);
        }

        [TestMethod]
        public void GetGlamOfClassifier_Success()
        {
            var conn = new Neo4jDB();
            GLAM glam = new GLAM("Fetched GLAM");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingGetGlamOfMe@BCCNeo4j.com";

            conn.addClassifier(classifier);

            GLAM fetchedG = conn.getGlamOfClassifier(classifier.email);

            Assert.AreEqual(glam.name, fetchedG.name);

            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void GetAllGlams_Exists()
        {
            var conn = new Neo4jDB();

            List<GLAM> resGlam = conn.getAllGlams();

            // TODO: sample data should have a GLAM called AAAAAAAA
            // and therefore it would be the first one alphabetically.
            Assert.AreNotEqual(0, resGlam.Count);
            Assert.AreEqual("Achaemenid GLAM", resGlam[0].name);
        }

        [TestMethod]
        public void GetClassifiableById_IsClassified_IdExists()
        {
            var conn = new Neo4jDB();

            String searchById = "US National Parks Service_Adze Blade";
            Classifiable classifiedWithGoodId = conn.getClassifiableById(searchById);

            Assert.IsNotNull(classifiedWithGoodId);
            Assert.AreEqual(searchById.ToString(), classifiedWithGoodId.id);
            Assert.AreEqual("Adze Blade", classifiedWithGoodId.name);
            Assert.AreEqual(6, classifiedWithGoodId.conceptStr.terms.Count);
            Assert.AreEqual("(blade)(of)(Tool)(for)(carving)(wood)", classifiedWithGoodId.conceptStr.ToString());
            Assert.IsNotNull(classifiedWithGoodId.owner.email);
            Assert.AreEqual("US National Parks Service", classifiedWithGoodId.owner.getOrganizationName());
        }

        [TestMethod]
        public void GetClassifiableById_NotClassified_IdExists()
        {
            var conn = new Neo4jDB();

            String searchById = "US National Parks Service_Atlatl Foreshaft";
            Classifiable notClassifiedWithGoodId = conn.getClassifiableById(searchById);

            Assert.IsNotNull(notClassifiedWithGoodId);
            Assert.AreEqual(searchById.ToString(), notClassifiedWithGoodId.id);
            Assert.AreEqual(0, notClassifiedWithGoodId.conceptStr.terms.Count);
            Assert.AreEqual("", notClassifiedWithGoodId.conceptStr.ToString());
        }

        [TestMethod]
        public void GetClassifiableById_DoesNotExist()
        {
            var conn = new Neo4jDB();

            String searchById = "909090";
            Classifiable doesNotExistClassifiable = conn.getClassifiableById(searchById);

            Assert.IsNull(doesNotExistClassifiable);
        }

        [TestMethod]
        public void GetClassiablesByName_ExistsOne()
        {
            var conn = new Neo4jDB();

            string searchByName = "Adze Blade";
            ClassifiableCollection matchedNameClassifiable = conn.getClassifiablesByName(searchByName);

            Assert.IsNotNull(matchedNameClassifiable.data);
            Assert.AreEqual(1, matchedNameClassifiable.data.Count);
            Assert.AreEqual(searchByName, matchedNameClassifiable.data[0].name);
            Assert.AreEqual(6, matchedNameClassifiable.data[0].conceptStr.terms.Count);
            Assert.AreEqual("(blade)(of)(Tool)(for)(carving)(wood)", matchedNameClassifiable.data[0].conceptStr.ToString());
        }

        [TestMethod]
        public void GetClassifiablesByName_DoesNotExist()
        {
            var conn = new Neo4jDB();

            string searchByName = "909090";
            ClassifiableCollection noMatchedName = conn.getClassifiablesByName(searchByName);

            Assert.IsNotNull(noMatchedName);
            Assert.AreEqual(0, noMatchedName.data.Count);
        }

        [TestMethod]
        public void GetClassifiablesByAlpha_LetterACaseInsensitive()
        {
            var conn = new Neo4jDB();
            
            // Get using uppercase and verify that each one starts with with 'A' or 'a'
            ClassifiableCollection resultsUpperStart = conn.getClassifiablesByAlphaGroup('A');
            foreach (Classifiable c in resultsUpperStart.data)
            {
                bool check = c.name[0] == 'A' || c.name[0] == 'a';
                Assert.IsTrue(check);
            }

            // Get using lowercase and verify that each one starts with with 'A' or 'a'
            ClassifiableCollection resultsLowerStart = conn.getClassifiablesByAlphaGroup('a');
            foreach (Classifiable c in resultsLowerStart.data)
            {
                bool check = c.name[0] == 'A' || c.name[0] == 'a';
                Assert.IsTrue(check);
            }

            // Whether it is upper or lower case, should return the same numebr of results.
            Assert.AreEqual(resultsLowerStart.data.Count, resultsUpperStart.data.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
            "This is not a letter of the alphabet.")]
        public void GetClassifiablesByAlpha_Number_ThrowsException()
        {
            var conn = new Neo4jDB();
            ClassifiableCollection results = conn.getClassifiablesByAlphaGroup('2');
        }

        [TestMethod]
        public void GetClassifiables_SomeExist()
        {
            var conn = new Neo4jDB();

            GLAM glam = new GLAM("GettingClassifiables01");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingGetMyClassifiables@BCCNeo4j.com";

            Term termTool = new Term
            {
                rawTerm = "Tool",
            };

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term> 
                { 
                    termTool, 
                }
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyAdd01",
                name = "dummyAdd01",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            conn.addClassifier(classifier);

            Classifiable result = conn.addClassifiable(newClassifiable);

            // Create the second classifiable
            Classifiable newClassifiable2 = newClassifiable;
            newClassifiable2.name = "dummyAdd02";
            newClassifiable2.id = glam.name + "_" + newClassifiable2.name;

            // Add the second classifiable
            Classifiable result2 = conn.addClassifiable(newClassifiable2);

            ClassifiableCollection resCollection = conn.getClassifiables(classifier);

            Assert.AreEqual(2, resCollection.data.Count);

            // Clean up
            conn.deleteClassifiable(result);
            conn.deleteClassifiable(result2);
            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }
        

        [TestMethod]
        public void GetRecentlyClassified_Yours_AfterTwoAdds()
        {
            // Get your recently test steps:
            // 1) Classify something 
            // 2) Get recently classified
            // 3) Classify again
            // 4) Get recently classified and should be in order...
            var conn = new Neo4jDB();

            GLAM glam = new GLAM("Sample");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingRecent@BCCNeo4j.com";

            Term termTool = new Term
            {
                rawTerm = "Tool",
            };

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term> 
                { 
                    termTool, 
                }
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyAdd01",
                name = "dummyAdd01",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            conn.addClassifier(classifier);

            // Add the first classifiable and get the recent results
            Classifiable result = conn.addClassifiable(newClassifiable);
            ClassifiableCollection recent1 = conn.getRecentlyClassified(classifier.email);

            // Test to make sure we got one result for that new classifier
            Assert.AreEqual(1, recent1.data.Count);
            Assert.AreEqual(newClassifiable.name, recent1.data[0].name);

            // Create the second classifiable
            Classifiable newClassifiable2 = newClassifiable;
            newClassifiable2.name = "dummyAdd02";
            newClassifiable2.id = glam.name + "_" + newClassifiable2.name;

            // Add the second classifiable
            Classifiable result2 = conn.addClassifiable(newClassifiable2);
            ClassifiableCollection recent2 = conn.getRecentlyClassified(classifier.email);

            // Test to make sure we get two classifiables and the one we just added is first
            Assert.AreEqual(2, recent2.data.Count);
            Assert.AreEqual(result2.name, recent2.data[0].name);
            Assert.AreEqual(result.name, recent2.data[1].name);

            // Clean up
            conn.deleteClassifiable(result);
            conn.deleteClassifiable(result2);
            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void GetRecentlyClassified_YoursOnly()
        {
            // Steps:
            // 1) ClassifierA adds Classifiable A_1, perm GLAM
            //      Recent A: A_1
            // 2) ClassifierA adds Classifiable A_2, perm OwnerOnly
            //      Recent A: A_2, A_1
            // 3) Classifier B adds Classifiable B_1, perm GLAM
            //      Recent A: B_1, A_2, A_1
            // 4) Classifier A adds Classifiable A_3, perm GLAM
            //      Recent A: A_3, A_2, A_1
            //      Recent B: B_1
            GLAM glam = new GLAM("Recent A vs B");

            Classifier classifierA = new Classifier(glam);
            classifierA.email = "testingRecentA@BCCNeo4j.com";

            Classifier classifierB = new Classifier(glam);
            classifierB.email = "testingRecentB@BCCNeo4j.com";

            Term termTool = new Term
            {
                rawTerm = "Tool",
            };

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term> 
                { 
                    termTool, 
                }
            };

            Classifiable A1GLAM = new Classifiable
            {
                id = glam.name + "_" + "A1 GLAM",
                name = "A1 GLAM",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifierA,
                conceptStr = conStr,
            };

            Classifiable A2OwnerOnly = new Classifiable
            {
                id = glam.name + "_" + "A2 OwnerOnly",
                name = "A2 OwnerOnly",
                url = "dummyURL",
                perm = Classifiable.Persmission.OwnerOnly.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifierA,
                conceptStr = conStr,
            };

            Classifiable A3GLAM = new Classifiable
            {
                id = glam.name + "_" + "A3 GLAM",
                name = "A3 GLAM",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifierA,
                conceptStr = conStr,
            };

            Classifiable B1GLAM = new Classifiable
            {
                id = glam.name + "_" + "B1 GLAM",
                name = "B1 GLAM",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifierB,
                conceptStr = conStr,
            };

            var conn = new Neo4jDB();

            conn.addClassifier(classifierA);
            conn.addClassifier(classifierB);

            // Add the Classifiables in this order
            Classifiable resA1 = conn.addClassifiable(A1GLAM);
            Classifiable resA2 = conn.addClassifiable(A2OwnerOnly);
            Classifiable resB1 = conn.addClassifiable(B1GLAM);
            Classifiable resA3 = conn.addClassifiable(A3GLAM);

            ClassifiableCollection recentA = conn.getRecentlyClassified(classifierA.email);
            ClassifiableCollection recentB = conn.getRecentlyClassified(classifierB.email);

            // Verify Classifier A's recently modified
            Assert.AreEqual(3, recentA.data.Count);
            Assert.AreEqual(A3GLAM.name, recentA.data[0].name);
            Assert.AreEqual(A2OwnerOnly.name, recentA.data[1].name);
            Assert.AreEqual(A1GLAM.name, recentA.data[2].name);

            // Verify Classifier B's recently modified
            Assert.AreEqual(1, recentB.data.Count);
            Assert.AreEqual(B1GLAM.name, recentB.data[0].name);

            // Clean up
            conn.deleteClassifiable(A1GLAM);
            conn.deleteClassifiable(A2OwnerOnly);
            conn.deleteClassifiable(A3GLAM);
            conn.deleteClassifiable(B1GLAM);
            conn.deleteClassifier(classifierA);
            conn.deleteClassifier(classifierB);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void GetAllUnclassified_YourOwn_Exists()
        {
            GLAM glam = new GLAM("US National Parks Service");
            Classifier classifier = new Classifier(glam);
            classifier.email = "user1@USNationalParks.com";

            var conn = new Neo4jDB();

            ClassifiableCollection unclassifieds = conn.getAllUnclassified(classifier.email);

            // TODO: fix: Bad test without sample data, but will do for now
            Assert.AreNotEqual(0, unclassifieds.data.Count);

            // check cases:
            // not all may have a null constr
            // some may not have a ConceptString
            foreach (var unclassified in unclassifieds.data)
            {
                Assert.AreEqual(0, unclassified.conceptStr.terms.Count);
                Assert.AreEqual("", unclassified.conceptStr.ToString());
            }
        }

        [TestMethod]
        public void GetAllUnclassified_NotYours_Exists()
        {
            GLAM glam = new GLAM("US National Parks Service");
            Classifier classifier = new Classifier(glam);
            classifier.email = "user2@USNationalParks.com";

            var conn = new Neo4jDB();

            conn.addClassifier(classifier);

            ClassifiableCollection unclassifieds = conn.getAllUnclassified(classifier.email);

            // TODO: fix: Bad test without sample data, but will do for now
            Assert.AreNotEqual(0, unclassifieds.data.Count);

            // check cases:
            // not all may have a null constr
            // some may not have a ConceptString
            // Since we just added the classifier and didn't add any classifiables to them,
            // We can check that all the Classifiables they should have should not
            // have the permission OwnerOnly.
            foreach (var unclassified in unclassifieds.data)
            {
                Assert.AreNotEqual(Classifiable.Persmission.OwnerOnly,
                    unclassified.perm);
                Assert.AreEqual(0, unclassified.conceptStr.terms.Count);
                Assert.AreEqual("", unclassified.conceptStr.ToString());
            }

            conn.deleteClassifier(classifier);
        }

        [TestMethod]
        public void GettAllUnclassified_YoursAndOthersWithPerm()
        {
            // Steps:
            // 1) ClassifierA adds Classifiable A_1, perm GLAM
            // 2) ClassifierA adds Classifiable A_2, perm OwnerOnly
            // 3) ClassifierB adds Classifiable B_1, perm GLAM
            //      ListLen A: 3
            //      ListLen B: 2
            GLAM glam = new GLAM("Recent Uncclassified YoursandOthers");

            Classifier classifierA = new Classifier(glam);
            classifierA.email = "testingUnclassedA@BCCNeo4j.com";

            Classifier classifierB = new Classifier(glam);
            classifierB.email = "testingUnclassedB@BCCNeo4j.com";

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term>(),
            };

            Classifiable A1GLAM = new Classifiable
            {
                id = glam.name + "_" + "A1 GLAM",
                name = "A1 GLAM",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Unclassified.ToString(),
                owner = classifierA,
                conceptStr = conStr,
            };

            Classifiable A2OwnerOnly = new Classifiable
            {
                id = glam.name + "_" + "A2 OwnerOnly",
                name = "A2 OwnerOnly",
                url = "dummyURL",
                perm = Classifiable.Persmission.OwnerOnly.ToString(),
                status = Classifiable.Status.Unclassified.ToString(),
                owner = classifierA,
                conceptStr = conStr,
            };

            Classifiable B1GLAM = new Classifiable
            {
                id = glam.name + "_" + "B1 GLAM",
                name = "B1 GLAM",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Unclassified.ToString(),
                owner = classifierB,
                conceptStr = conStr,
            };

            var conn = new Neo4jDB();

            // Add the two classifiers (since we need the glams)
            conn.addClassifier(classifierA);
            conn.addClassifier(classifierB);

            // Add the Classifiables in this order
            Classifiable resA1 = conn.addClassifiable(A1GLAM);
            Classifiable resA2 = conn.addClassifiable(A2OwnerOnly);
            Classifiable resB1 = conn.addClassifiable(B1GLAM);

            ClassifiableCollection recentA = conn.getAllUnclassified(classifierA.email);
            ClassifiableCollection recentB = conn.getAllUnclassified(classifierB.email);

            Assert.AreEqual(3, recentA.data.Count);
            Assert.AreEqual(2, recentB.data.Count);

            // Clean up
            conn.deleteClassifiable(resA1);
            conn.deleteClassifiable(resA2);
            conn.deleteClassifiable(resB1);
            conn.deleteClassifier(classifierA);
            conn.deleteClassifier(classifierB);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void GetAllUnclassified_OtherTypes()
        {
            // We have new status types to worry about; pending, need help(?),
            // etc. ATM the function only gets those with status of "Unclassified".
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void GettAllUnclassified_HasNone()
        {
            GLAM glam = new GLAM("NonExisting");
            Classifier classifier = new Classifier(glam);
            classifier.email = "userDoesNotExistUnclassified@USNationalParks.com";

            var conn = new Neo4jDB();

            ClassifiableCollection unclassifieds = conn.getAllUnclassified(classifier.email);

            Assert.AreEqual(0, unclassifieds.data.Count);
        }

        [TestMethod]
        public void AddClassifiable_Succeed()
        {
            var conn = new Neo4jDB();

            GLAM glam = new GLAM("AddingClassifiableSuccess");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testing1@BCCNeo4j.com";

            Term termTool = new Term
            {
                rawTerm = "Tool",
            };

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term> 
                { 
                    termTool, 
                }
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyName1",
                name = "dummyName1",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            conn.addClassifier(classifier);

            Classifiable result = conn.addClassifiable(newClassifiable);

            Assert.AreEqual(newClassifiable.id, result.id);
            Assert.AreEqual(newClassifiable.name, result.name);
            Assert.AreEqual(newClassifiable.url, result.url);
            Assert.AreEqual(newClassifiable.perm, result.perm);
            Assert.AreEqual(newClassifiable.status, result.status);
            Assert.AreEqual(newClassifiable.owner.email, result.owner.email);

            Assert.AreEqual(newClassifiable.conceptStr.ToString(),
                result.conceptStr.ToString());

            Assert.AreEqual(newClassifiable.conceptStr.terms.Count,
                            result.conceptStr.terms.Count);

            for (int i = 0; i < newClassifiable.conceptStr.terms.Count; i++)
            {
                Assert.AreEqual(newClassifiable.conceptStr.terms[i].ToString(),
                                result.conceptStr.terms[i].ToString());
            }

            conn.deleteClassifiable(result);
            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException),
            "Classifiable information missing or Classifier email was not set.")]
        public void AddClassifiable_NoClassifier_ThrowNullReferenceException()
        {
            var conn = new Neo4jDB();

            GLAM glam = new GLAM("AddClassifiableWnoClassifier");

            Term termTool = new Term
            {
                rawTerm = "Tool",
            };

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term> 
                { 
                    termTool, 
                }
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyName2",
                name = "dummyName2",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                conceptStr = conStr,
            };

            Classifiable result = conn.addClassifiable(newClassifiable);
        }

        [TestMethod]
        public void AddClassifiable_AlreadyExists_ThrowNeoException()
        {
            var conn = new Neo4jDB();

            GLAM glam = new GLAM("AddClassifier But Exists");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingAlreadyExists@BCCNeo4j.com";

            Term termTool = new Term
            {
                rawTerm = "Tool",
            };

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term> 
                { 
                    termTool, 
                }
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyName3",
                name = "dummyName3",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            conn.addClassifier(classifier);
            Classifiable result = conn.addClassifiable(newClassifiable);
            result.url = "anotherDummyUrl";

            // Try adding another Classifiable, but with the same id. Should
            // throw an exception.
            // TODO: once we have setup and clean up methods, remove this try-catch
            // so that the clean doesn't have to be handled in this test function
            try
            {
                Classifiable result2 = conn.addClassifiable(result);
            }
            catch (Exception)
            {
            }
            conn.deleteClassifiable(result);
            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void AddClassifiable_TermsDoNotExist()
        {
            var conn = new Neo4jDB();

            GLAM glam = new GLAM("AddClassifiable ButBadTerms");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingNoTermsExist@BCCNeo4j.com";

            Term termTooool = new Term
            {
                rawTerm = "Tooool",
            };

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term> 
                { 
                    termTooool, 
                }
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyName4",
                name = "dummyName4",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            conn.deleteClassifiable(newClassifiable);
            conn.deleteClassifier(classifier);

            conn.addClassifier(classifier);

            try
            {
                Classifiable result = conn.addClassifiable(newClassifiable);
                conn.deleteClassifiable(result);
            }
            catch (Exception) { }

            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void AddClassifiable_WithNoTerms()
        {
            var conn = new Neo4jDB();

            GLAM glam = new GLAM("AddingWithNoTerms");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingNoTerms@BCCNeo4j.com";

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term>(),
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyName5",
                name = "dummyName5",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            conn.addClassifier(classifier);

            Classifiable result = conn.addClassifiable(newClassifiable);

            Assert.AreEqual(0, result.conceptStr.terms.Count);
            Assert.AreEqual("", result.conceptStr.ToString());

            conn.deleteClassifiable(result);
            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void UpdateClassifiable_YoursSimple_Success()
        {
            GLAM glam = new GLAM("Updating GLAM");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingUpdateSimple@BCCNeo4j.com";

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term>(),
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyName1",
                name = "dummyName1",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Unclassified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            // Add the classifiable
            var conn = new Neo4jDB();

            conn.addClassifier(classifier);

            Classifiable addedClassifiable = conn.addClassifiable(newClassifiable);

            // Make changes and update
            newClassifiable.name = "newName";
            newClassifiable.url = "newUrl";
            newClassifiable.perm = Classifiable.Persmission.OwnerOnly.ToString();
            
            Classifiable updatedClassifiable = conn.updateClassifiable(addedClassifiable, newClassifiable, classifier);

            // Make checks.
            // TODO: call getById and compare that way instead?
            Assert.AreEqual(String.Format("{0}_{1}", glam.ToString(), newClassifiable.name), 
                            updatedClassifiable.id);
            Assert.AreEqual(newClassifiable.name, updatedClassifiable.name);
            Assert.AreEqual(newClassifiable.url, updatedClassifiable.url);
            Assert.AreEqual(newClassifiable.perm, updatedClassifiable.perm);
            Assert.AreEqual(newClassifiable.status, updatedClassifiable.status);

            Assert.AreEqual(newClassifiable.conceptStr.ToString(),
                            updatedClassifiable.conceptStr.ToString());

            // Cleanup
            conn.deleteClassifiable(updatedClassifiable);
            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void UpdateClassifiable_AnothersSimple_Success()
        {
            GLAM glam = new GLAM("Updating GLAM");

            Classifier ownerClassifier = new Classifier(glam);
            ownerClassifier.email = "testingUpdateSimpleOwner@BCCNeo4j.com";

            Classifier editingClassifier = new Classifier(glam);
            editingClassifier.email = "testingUpdateSimpleAnother@BCCNeo4j.com";

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term>(),
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyName1",
                name = "dummyName1",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Unclassified.ToString(),
                owner = ownerClassifier,
                conceptStr = conStr,
            };

            // Add the classifiable
            var conn = new Neo4jDB();

            conn.addClassifier(ownerClassifier);
            conn.addClassifier(editingClassifier);

            Classifiable addedClassifiable = conn.addClassifiable(newClassifiable);

            // Make changes and update
            newClassifiable.name = "newName";
            newClassifiable.url = "newUrl";
            newClassifiable.perm = Classifiable.Persmission.OwnerOnly.ToString();
            
            Classifiable updatedClassifiable = conn.updateClassifiable(addedClassifiable, newClassifiable, editingClassifier);

            // Make checks.
            // TODO: call getById and compare that way instead?
            Assert.AreEqual(String.Format("{0}_{1}", glam.ToString(), newClassifiable.name), 
                            updatedClassifiable.id);
            Assert.AreEqual(newClassifiable.name, updatedClassifiable.name);
            Assert.AreEqual(newClassifiable.url, updatedClassifiable.url);
            Assert.AreEqual(newClassifiable.perm, updatedClassifiable.perm);
            Assert.AreEqual(newClassifiable.status, updatedClassifiable.status);

            Assert.AreEqual(newClassifiable.conceptStr.ToString(),
                            updatedClassifiable.conceptStr.ToString());

            // Cleanup
            conn.deleteClassifiable(updatedClassifiable);
            conn.deleteClassifier(ownerClassifier);
            conn.deleteClassifier(editingClassifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void UpdateClassifiable_NotAllowed()
        {
            Assert.IsFalse(true);
        }

        [TestMethod]
        public void UpdateClassifiable_AddConStr_Success()
        {
            // Only the ConStr is affected; no other properties should be changed.
            GLAM glam = new GLAM("Updating GLAM");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingUpdateConStrAdd@BCCNeo4j.com";

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term>(),
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyNameCSAdd",
                name = "dummyNameCSAdd",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Unclassified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            // Add the classifiable
            var conn = new Neo4jDB();

            conn.addClassifier(classifier);

            Classifiable addedClassifiable = conn.addClassifiable(newClassifiable);

            // Make changes and update
            Term termTool = new Term
            {
                rawTerm = "Tool",
            };
            newClassifiable.conceptStr.terms.Add(termTool);

            Classifiable updatedClassifiable = conn.updateClassifiable(addedClassifiable, newClassifiable, classifier);

            // Make checks.
            // TODO: call getById and compare that way instead?
            Assert.AreEqual(String.Format("{0}_{1}", glam.ToString(), newClassifiable.name),
                            updatedClassifiable.id);
            Assert.AreEqual(newClassifiable.name, updatedClassifiable.name);
            Assert.AreEqual(newClassifiable.url, updatedClassifiable.url);
            Assert.AreEqual(newClassifiable.perm, updatedClassifiable.perm);
            Assert.AreEqual(newClassifiable.status, updatedClassifiable.status);

            Assert.AreEqual(1, newClassifiable.conceptStr.terms.Count);
            Assert.AreEqual(newClassifiable.conceptStr.ToString(),
                            updatedClassifiable.conceptStr.ToString());

            // Cleanup
            conn.deleteClassifiable(updatedClassifiable);
            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void UpdateClassifiable_RemoveConStr_Success()
        {
            // Only the ConStr is affected; no other properties should be changed.
            GLAM glam = new GLAM("Updating GLAM");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingUpdateConStrRemove@BCCNeo4j.com";

            // Make changes and update
            Term termTool = new Term
            {
                rawTerm = "Tool",
            };

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term> 
                { 
                    termTool,
                },
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyNameCSrm",
                name = "dummyNameCSrm",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Unclassified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            // Add the classifiable
            var conn = new Neo4jDB();

            conn.deleteClassifiable(newClassifiable);
            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);

            conn.addClassifier(classifier);

            Classifiable addedClassifiable = conn.addClassifiable(newClassifiable);

            List<Term> noTerms = new List<Term>();
            newClassifiable.conceptStr.terms = noTerms;

            Classifiable updatedClassifiable = conn.updateClassifiable(addedClassifiable, newClassifiable, classifier);

            // Make checks.
            // TODO: call getById and compare that way instead?
            Assert.AreEqual(String.Format("{0}_{1}", glam.ToString(), newClassifiable.name),
                            updatedClassifiable.id);
            Assert.AreEqual(newClassifiable.name, updatedClassifiable.name);
            Assert.AreEqual(newClassifiable.url, updatedClassifiable.url);
            Assert.AreEqual(newClassifiable.perm, updatedClassifiable.perm);
            Assert.AreEqual(newClassifiable.status, updatedClassifiable.status);

            // Make sure the Concept String has no concept string
            Assert.AreEqual(0, updatedClassifiable.conceptStr.terms.Count);
            Assert.AreEqual(newClassifiable.conceptStr.ToString(),
                            updatedClassifiable.conceptStr.ToString());

            // Cleanup
            conn.deleteClassifiable(updatedClassifiable);
            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void UpdateClassifiable_UniqueIdViolation_ThrowException()
        {
            // Only the ConStr is affected; no other properties should be changed.
            GLAM glam = new GLAM("Updating GLAM");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingUpdateViolateId@BCCNeo4j.com";

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term>(),
            };

             Classifiable uniqueClassifiable = new Classifiable
            {
                id = glam.name + "_" + "unique",
                name = "unique",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Unclassified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            Classifiable changingClassifiable = new Classifiable
            {
                id = glam.name + "_" + "original",
                name = "original",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Unclassified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            // Add the classifiable
            var conn = new Neo4jDB();

            conn.addClassifier(classifier);

            Classifiable addedClassifiable = conn.addClassifiable(uniqueClassifiable);
            Classifiable toUpdateClassifiable = conn.addClassifiable(changingClassifiable);

            // Unqiue violation! Two classifiables can't have the same name and be in the same GLAM!
            changingClassifiable.name = addedClassifiable.name;

            try
            {
                Classifiable updatedClassifiable = conn.updateClassifiable(toUpdateClassifiable, changingClassifiable, classifier);
            }
            catch (Exception)
            {
                // TODO: use custom exceptions or something for unqiue id violation, etc.
            }

            // Cleanup
            conn.deleteClassifiable(addedClassifiable);
            conn.deleteClassifiable(toUpdateClassifiable);
            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void UpdateClassifiable_ImproperTerms_ThrowsException()
        {
            GLAM glam = new GLAM("UpdatingClassifiable ImproperTerms");
         
            Classifier classifier = new Classifier(glam);
            classifier.email = "testingUpdatingImproperTerms@BCCNeo4j.com";

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term>(),
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyName4",
                name = "dummyName4",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            var conn = new Neo4jDB();
            conn.addClassifier(classifier);
            Classifiable addedClassifiable = conn.addClassifiable(newClassifiable);

            // Make changes and update
            Term termTooool = new Term
            {
                rawTerm = "Tooool",
            };
            newClassifiable.conceptStr.terms.Add(termTooool);

            try
            {
                Classifiable updatedClassifiable = conn.updateClassifiable(addedClassifiable, newClassifiable, classifier);
                conn.deleteClassifiable(updatedClassifiable);
            }
            catch (Exception)
            {
                // TODO: better exception handling
            }

            conn.deleteClassifiable(addedClassifiable);
            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void GetUnclassified_UpdateClassifiablePermChange()
        {
            // ClassifierA has a Classifiable with perm GLAM
            // CLassifierB who is in the same GLAM, can see it at first
            // Then ClassifierA changes the perm to be OwnerOnly
            // ClassifierB can no longer see that Classifiable
            GLAM glam = new GLAM("Recent Unclassified Update Perm");

            Classifier classifierA = new Classifier(glam);
            classifierA.email = "testingEditUnclassedOwner@BCCNeo4j.com";

            Classifier classifierB = new Classifier(glam);
            classifierB.email = "testingEditUnclassedAnother@BCCNeo4j.com";

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term>(),
            };

            Classifiable changingClassifiable = new Classifiable
            {
                id = glam.name + "_" + "A1 GLAM",
                name = "A1 GLAM",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Unclassified.ToString(),
                owner = classifierA,
                conceptStr = conStr,
            };

            var conn = new Neo4jDB();

            // Add the two classifiers (since we need the glams)
            conn.addClassifier(classifierA);
            conn.addClassifier(classifierB);

            // Add the Classifiables in this order
            Classifiable originalClassifiable = conn.addClassifiable(changingClassifiable);

            ClassifiableCollection recentA = conn.getAllUnclassified(classifierA.email);
            ClassifiableCollection recentB = conn.getAllUnclassified(classifierB.email);

            Assert.AreEqual(1, recentA.data.Count);
            Assert.AreEqual(1, recentB.data.Count);

            // Change permission so that Classifier B can't see it anymore
            changingClassifiable.perm = Classifiable.Persmission.OwnerOnly.ToString();

            Classifiable changedClassifiable = conn.updateClassifiable(originalClassifiable, changingClassifiable, classifierA);

            recentA = conn.getAllUnclassified(classifierA.email);
            recentB = conn.getAllUnclassified(classifierB.email);

            Assert.AreEqual(1, recentA.data.Count);
            Assert.AreEqual(0, recentB.data.Count);

            // Clean up
            conn.deleteClassifiable(changedClassifiable);
            conn.deleteClassifier(classifierA);
            conn.deleteClassifier(classifierB);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void GetRecentlyClassified_UpdateClassifiableYours()
        {
            // Have classifiedA, classifiedB for a Classifier
            // Recently: B, A
            // Modify A
            // Recently: A, B
            GLAM glam = new GLAM("Recent Unclassified Update Yours");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingEditRecentOwner@BCCNeo4j.com";

            // Make changes and update
            Term termTool = new Term
            {
                rawTerm = "Tool",
            };

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term> 
                {
                    termTool,
                }
            };

            Classifiable classifiableA = new Classifiable
            {
                id = glam.name + "_" + "A",
                name = "A",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            Classifiable classifiableB = new Classifiable
            {
                id = glam.name + "_" + "B",
                name = "B",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            var conn = new Neo4jDB();

            conn.addClassifier(classifier);
            Classifiable addedA = conn.addClassifiable(classifiableA);
            Classifiable addedB = conn.addClassifiable(classifiableB);

            ClassifiableCollection recent = conn.getRecentlyClassified(classifier.email);

            // Recent order: B, A
            Assert.AreEqual(addedB.id, recent.data[0].id);
            Assert.AreEqual(addedA.id, recent.data[1].id);

            // Modify A so it becomes the most recent
            classifiableA.url = "easyToChange";
            conn.updateClassifiable(addedA, classifiableA, classifier);

            recent = conn.getRecentlyClassified(classifier.email);

            // Recent order: A, B
            Assert.AreEqual(addedA.id, recent.data[0].id);
            Assert.AreEqual(addedB.id, recent.data[1].id);

            conn.deleteClassifiable(recent.data[0]);
            conn.deleteClassifiable(recent.data[1]);
            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void DeleteClassifiable_Suceed()
        {
            GLAM glam = new GLAM("DeletingAClassifiable");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingToDel@BCCNeo4j.com";

            Term termWood = new Term
            {
                rawTerm = "wood",
            };

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term> 
                { 
                    termWood, 
                }
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyNameToDelete",
                name = "dummyNameToDelete",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            var conn = new Neo4jDB();

            conn.addClassifier(classifier);

            Classifiable result = conn.addClassifiable(newClassifiable);

            conn.deleteClassifiable(result);

            Classifiable isGone = conn.getClassifiableById(newClassifiable.id);
            Assert.IsNull(isGone);

            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void DeleteClassifiable_NoTerms_Succeed()
        {
            GLAM glam = new GLAM("DeleteClassifiable-NoTerms-Success");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingToDelNoTerms@BCCNeo4j.com";

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term>(),
            };

            Classifiable newClassifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyNameToDelete02",
                name = "dummyNameToDelete02",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            var conn = new Neo4jDB();

            conn.addClassifier(classifier);

            Classifiable result = conn.addClassifiable(newClassifiable);

            conn.deleteClassifiable(result);

            Classifiable isGone = conn.getClassifiableById(newClassifiable.id);
            Assert.IsNull(isGone);

            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void DeleteClassifiable_DoesNotExist()
        {
            GLAM glam = new GLAM("DeleteClassifiable DoesNotExist");

            Classifier classifier = new Classifier(glam);
            classifier.email = "testingToDel03@BCCNeo4j.com";

            Term termWood = new Term
            {
                rawTerm = "wood",
            };

            ConceptString conStr = new ConceptString
            {
                terms = new List<Term> 
                { 
                    termWood, 
                }
            };

            Classifiable classifiable = new Classifiable
            {
                id = glam.name + "_" + "dummyNameToDelete03",
                name = "dummyNameToDelete03",
                url = "dummyURL",
                perm = Classifiable.Persmission.GLAM.ToString(),
                status = Classifiable.Status.Classified.ToString(),
                owner = classifier,
                conceptStr = conStr,
            };

            var conn = new Neo4jDB();
            conn.addClassifier(classifier);
            conn.deleteClassifiable(classifiable);
            conn.deleteClassifier(classifier);
            conn.deleteGlam(glam);
        }

        [TestMethod]
        public void GetClassifiablesByConceptString_Ordered_ClassifiablesMatch()
        {
            var conn = new Neo4jDB();

            Term termWood = new Term
            {
                rawTerm = "wood",
            };

            Term termTool = new Term
            {
                rawTerm = "Tool",
            };

            Term termFor = new Term
            {
                rawTerm = "for",
            };

            // good where clause: 
            // WHERE t.rawTerm = "wood" OR t.rawTerm = "for" OR t.rawTerm = "Tool"
            // but put in ids and not rawTerms, since ids are unique....
            // Maybe. Unless we want all kinds of say, "Beautiful"
            ConceptString searchByConStr = new ConceptString
            {
                terms = new List<Term> 
                {
                    termWood, termTool, termFor,
                },
            };

            ClassifiableCollection matchedClassifiables = conn.getClassifiablesByConStr(searchByConStr, ordered: true);

            Assert.IsNotNull(matchedClassifiables);

            // Tests that results are in order of most matches in its own concept string
            int prevMatchCount = searchByConStr.terms.Count;
            int currMatchCount = 0;
            Classifiable prevClassifiable = new Classifiable
            {
                name = "first",
            };

            for (int i = 0; i < matchedClassifiables.data.Count; i++)
            {
                var classifiable = matchedClassifiables.data[i];

                foreach (Term t in searchByConStr.terms)
                {
                    if (classifiable.conceptStr.ToString().Contains(t.ToString()))
                    {
                        currMatchCount += 1;
                    }
                }

                Assert.AreNotEqual(0, currMatchCount,
                    String.Format("Classifiable has {0} matches at index: {1:D} in results",
                        classifiable.name, i));

                Assert.IsTrue(currMatchCount <= prevMatchCount,
                              String.Format("Index {4}: Current match: {0:D} for {2}, Previous match: {1:D} for {3}",
                                currMatchCount,
                                prevMatchCount,
                                classifiable.name,
                                prevClassifiable.name,
                                i));

                prevMatchCount = currMatchCount;
                prevClassifiable = classifiable;
                currMatchCount = 0;
            }
        }

        [TestMethod]
        public void GetClassifiablesByConceptString_Unordered_ClassifiablesMatch()
        {
            var conn = new Neo4jDB();

            Term termWood = new Term
            {
                rawTerm = "wood",
            };

            Term termTool = new Term
            {
                rawTerm = "Tool",
            };

            Term termFor = new Term
            {
                rawTerm = "for",
            };

            // good where clause: 
            // WHERE t.rawTerm = "wood" OR t.rawTerm = "for" OR t.rawTerm = "Tool"
            // but put in ids and not rawTerms, since ids are unique....
            // Maybe. Unless we want all kinds of say, "Beautiful"
            ConceptString searchByConStr = new ConceptString
            {
                terms = new List<Term> 
                {
                    termWood, termTool, termFor,
                },
            };


            ClassifiableCollection matchedClassifiables = conn.getClassifiablesByConStr(searchByConStr);

            Assert.IsNotNull(matchedClassifiables);

            // Only tests that each Classifiable's ConceptString does contain at least one of the
            // terms that was searched by.
            foreach (var classifiable in matchedClassifiables.data)
            {
                bool test = classifiable.conceptStr.ToString().Contains(termWood.rawTerm) ||
                            classifiable.conceptStr.ToString().Contains(termTool.rawTerm) ||
                            classifiable.conceptStr.ToString().Contains(termFor.rawTerm);
                Assert.IsTrue(test);
            }

        }

        [TestMethod]
        public void GetTermByRaw_Exists()
        {
            var conn = new Neo4jDB();

            string searchByRawTerm = "Art";
            Term artTerm = conn.getTermByRaw(searchByRawTerm);

            Assert.AreEqual(searchByRawTerm, artTerm.rawTerm);
            Assert.AreEqual("art", artTerm.lower);
            Assert.IsNotNull(artTerm.id);
            Assert.IsNotNull(artTerm.subTerms);
        }

        [TestMethod]
        public void GetTermByLower_Exists()
        {
            var conn = new Neo4jDB();

            string searchByLowerTerm = "art";
            Term term1 = conn.getTermByLower(searchByLowerTerm);

            Assert.AreEqual(searchByLowerTerm, term1.lower);
            Assert.AreEqual("Art", term1.rawTerm);
            Assert.IsNotNull(term1.id);
            Assert.IsNotNull(term1.subTerms);
        }

        [TestMethod]
        public void GetChildrenOfTerm_RootTerm_Exists()
        {
            var conn = new Neo4jDB();

            string rootRaw = "Top Object";
            int NOT_FOUND = -1;

            // These are the rawTerms that are one depth away from the root.
            List<string> rootSubTerms = new List<string>
            {
                "Art", 
                "Biological Entities", 
                "Celestial Objects",
                "Changes within a thing", 
                "Culture", 
                "Economy",
                "Flora and Fauna", 
                "Genetic Predisposition", 
                "Health and population", 
                "Individual differences",
                "Mathematical Concepts", 
                "Molecules and Elements",
                "Non-human Environment", 
                "Politics",
                "Properties, Qualities", 
                "Rocks", 
                "Social Structure",
                "Technology and Science", 
                "Waves and Particles",
            };

            Term rootTerm = conn.getTermByRaw(rootRaw);

            rootTerm.subTerms = conn.getChildrenOfTerm(rootTerm);

            // Make sure they're the same length
            Assert.AreEqual(rootSubTerms.Count, rootTerm.subTerms.Count);

            foreach (string expectedSubTermStr in rootSubTerms)
            {
                Term expectedSubTerm = new Term
                {
                    rawTerm = expectedSubTermStr,
                };
                Assert.AreNotEqual(NOT_FOUND, rootTerm.hasSubTerm(expectedSubTerm));
            }
        }

        [TestMethod]
        public void GetChildrenOfTerm_NoChildren()
        {
            var conn = new Neo4jDB();
            string searchRawTerm = "Biology";

            Term biologyTerm = conn.getTermByRaw(searchRawTerm);
            Assert.IsNotNull(biologyTerm);

            biologyTerm.subTerms = conn.getChildrenOfTerm(biologyTerm);
            Assert.AreEqual(0, biologyTerm.subTerms.Count);
        }

        [TestMethod]
        public void SearchForNonTerm_ManyFunctions_ReturnNull()
        {
            var conn = new Neo4jDB();

            Term NotExistTerm = new Term
            {
                rawTerm = "I am NOT a Term",
                subTerms = new List<Term>(),
            };

            Term resTerm = conn.getTermByRaw(NotExistTerm.rawTerm);
            Assert.IsNull(resTerm);

            List<Term> resTerm2 = conn.getChildrenOfTerm(NotExistTerm);
            Assert.AreEqual(0, resTerm2.Count);

            Term resTerm3 = conn.getBccFromTermWithDepth(NotExistTerm, 0);
            Assert.IsNull(resTerm3);

            Term resTerm4 = conn.getBccFromTermWithDepth(NotExistTerm, 2);
            Assert.IsNull(resTerm4);

            Term resTerm5 = conn.getBccFromTerm(NotExistTerm);
            Assert.IsNull(resTerm5);
        }

        [TestMethod]
        public void GetBccFromTermWithDepth_NaturalSciencesMaxDepth_Exists()
        {
            var conn = new Neo4jDB();

            int MAX_DEPTH = -1;
            int NOT_FOUND = -1;

            string searchRawTerm = "Natural Sciences";
            List<string> expectedNaturalSciencesSubTermStr = new List<string>
            {
                "Biology",
                "Chemistry",
                "Computer science",
                "Geology",
                "Physics",
            };

            Term naturalSciencesTerm = conn.getTermByRaw(searchRawTerm);

            Term naturalSciencesTree = conn.getBccFromTermWithDepth(naturalSciencesTerm,
                                                                    MAX_DEPTH);

            Assert.AreEqual(searchRawTerm, naturalSciencesTree.rawTerm);
            Assert.AreEqual(expectedNaturalSciencesSubTermStr.Count,
                            naturalSciencesTree.subTerms.Count);

            // Expected results, not in a particular order.
            // Natural Sciences - Biology, 
            //                  - Chemistry
            //                  - Physics
            //                      - Astronomy
            //                  - Computer Science
            //                  - Geology
            foreach (string rawSubTermStr in expectedNaturalSciencesSubTermStr)
            {
                Term expectedSubTerm = new Term
                {
                    rawTerm = rawSubTermStr,
                };

                int index = naturalSciencesTree.hasSubTerm(expectedSubTerm);
                Assert.AreNotEqual(NOT_FOUND, index);

                // Physics has Astronmy underneath it.
                if (searchRawTerm == "Physics")
                {
                    Term astronomy = new Term
                    {
                        rawTerm = "Astronomy"
                    };
                    Assert.AreNotEqual(NOT_FOUND, naturalSciencesTree.subTerms[index].hasSubTerm(astronomy));
                }
            }
        }

        [TestMethod]
        public void GetBccFromTermWithDepth_WavesAndParticlesDepth2_OnlyHasDepth2()
        {
            var conn = new Neo4jDB();

            int depth = 2;
            int EXPECTED_NUM_SUBTERMS = 6;

            string searchRawTerm = "Waves and Particles";

            Term wavesParticlesTerm = conn.getTermByRaw(searchRawTerm);

            Term wavesParticlesRootD2 = conn.getBccFromTermWithDepth(wavesParticlesTerm, depth);

            // "Waves and Particles" has subTerms, and each of those subTerms
            // also has subTerms. Makes it a bit easier to test that we only get 
            // a depth of 2 for results when the subTerm results are (currently) 
            // not organized alphabetically. 
            Assert.AreEqual(searchRawTerm, wavesParticlesRootD2.rawTerm);
            Assert.AreEqual(EXPECTED_NUM_SUBTERMS, wavesParticlesRootD2.subTerms.Count);
            Assert.AreNotEqual(0, wavesParticlesRootD2.subTerms[0].subTerms.Count);
            Assert.AreEqual(0, wavesParticlesRootD2.subTerms[0].subTerms[0].subTerms.Count);
        }

        [TestMethod]
        public void GetRootTerm_BccRoot_Exists()
        {
            var conn = new Neo4jDB();

            Term bccRootTerm = conn.getRootTerm();

            Assert.IsNotNull(bccRootTerm);
            Assert.AreEqual("bccRoot", bccRootTerm.id);
            Assert.AreEqual("bccroot", bccRootTerm.lower);
            Assert.AreEqual("BccRoot", bccRootTerm.rawTerm);
            Assert.IsNotNull(bccRootTerm.subTerms);
            Assert.AreEqual(0, bccRootTerm.subTerms.Count);
        }

        [TestMethod]
        public void GetBccFromRootWithDepth_BccRootD1_Exists()
        {
            int depth1 = 1;

            Term rootTermWithRawTerm = new Term
            {
                rawTerm = "ROOT TERM",
            };

            Term topObjPropertyWithRawTerm = new Term
            {
                rawTerm = "TOP OBJECT PROPERTY",
            };

            Term notRealTermWithRawTerm = new Term
            {
                rawTerm = "Not a Real Term",
            };

            var conn = new Neo4jDB();

            Term bccRootTermD1 = conn.getBccFromRootWithDepth(depth1);

            Assert.IsNotNull(bccRootTermD1);
            Assert.AreEqual("bccroot", bccRootTermD1.lower);
            Assert.IsNotNull(bccRootTermD1.subTerms.Count);
            Assert.IsNotNull(bccRootTermD1.hasSubTerm(rootTermWithRawTerm));
            Assert.IsNotNull(bccRootTermD1.hasSubTerm(topObjPropertyWithRawTerm));
            Assert.IsNotNull(bccRootTermD1.hasSubTerm(notRealTermWithRawTerm));
        }

        [TestMethod]
        public void TestAddTerm()
        {
            var conn = new Neo4jDB();

            Term t1 = new Term
            {
                id = "TEST_1",
                rawTerm = "Test 1",
                lower = "test 1",
                subTerms = new List<Term>()
            };

            for (int i = 0; i < 3; i++)
            {
                Term subterm = new Term
                {
                    id = "CHILD_" + i,
                    rawTerm = "Child " + i,
                    lower = "child " + i
                };

                t1.subTerms.Add(subterm);
            }

            Assert.AreEqual<int>(4, conn.addTerm(t1, null));

            // Cleanup the db changes we made.
            foreach (Term subterm in t1.subTerms)
            {
                Assert.AreEqual<int>(2, conn.delTermFORCE(subterm));
            }
            Assert.AreEqual<int>(2, conn.delTermFORCE(t1));
        }

        [TestMethod]
        public void TestMoveTerm()
        {
            var conn = new Neo4jDB();

            // Create 2 terms.
            Term t1 = new Term
            {
                id = "TEST_1",
                rawTerm = "Test 1",
                lower = "test 1"
            };
            Term t2 = new Term
            {
                id = "TEST_2",
                rawTerm = "Test 2",
                lower = "test 2"
            };

            // Add both to the root node.
            Assert.AreEqual<int>(1, conn.addTerm(t1, null));
            Assert.AreEqual<int>(1, conn.addTerm(t2, null));

            // Move t1 to be a subterm of t2
            Assert.AreEqual<int>(2, conn.moveTerm(t1, t2));

            // Clean up the mess
            Assert.AreEqual<int>(2, conn.delTermFORCE(t1));
            Assert.AreEqual<int>(2, conn.delTermFORCE(t2));
        }

        [TestMethod]
        public void TestRenameTerm()
        {
            var conn = new Neo4jDB();

            // Create a new node to test on.
            Term t1 = new Term
            {
                id = "TEST_1",
                rawTerm = "Hello!",
                lower = "hello"
            };

            conn.addTerm(t1, null);
            Assert.AreEqual(t1.rawTerm, conn.getTermByRaw("Hello!").rawTerm);

            // Rename the term.
            Assert.IsTrue(conn.renameTerm(t1, "World!"));

            // Check the op was successful.
            Term checkMe = conn.getTermByRaw("World!");
            Assert.AreEqual("World!", checkMe.rawTerm);
            Assert.AreEqual("world!", checkMe.lower);

            // Cleanup the changes we made to the db.
            t1.rawTerm = "World!";
            t1.lower = "world";

            conn.delTermFORCE(t1);
        }

        [TestMethod]
        public void testValidateDelete()
        {
            var conn = new Neo4jDB();

            // Create a test node with no children
            Term t1 = new Term
            {
                id = "TEST_1",
                rawTerm = "Test 1",
                lower = "test 1"
            };

            // Add that to the db.
            conn.addTerm(t1, null);

            // We expect to be able to delete that safely.
            Assert.IsTrue(conn.validateDeleteTerm(t1));

            // Delete the term we just added to the db.
            conn.delTermFORCE(t1);

            // Check some random term in the middle of the db.
            Term t2 = new Term
            {
                id = "http://www.w3.org/2002/07/owl#topObjectProperty"
            };

            // We expect to not be able to delete this.
            Assert.IsFalse(conn.validateDeleteTerm(t2));
        }

        [TestMethod]
        public void TestDelTermPreview()
        {
            var conn = new Neo4jDB();

            // See what happens when we try to delete the term "Heating" from the database.
            // Note that this could fail when the db is updated.
            Term t1 = new Term
            {
                id = "tmpId-protection",
            };

            var result = conn.delTermPREVIEW(t1);

            // Correct values last I checked XD.
            // The correct values can be found using the query:
            //      match (a:Term{id:"tmpId-protection"})-[r:HAS_TERM]-(b) return count(b)
            Assert.AreEqual<int>(1, result.stringsAffected.Count);

            //      match (a:ConceptString{terms:"(protection)(for)(war)"})-[r:HAS_CONSTR]-(b) count(b)
            Assert.AreEqual<int>(1, result.classifiablesAffected.Count);
        }

        [TestMethod]
        public void TestCreateNotificationsRaw()
        {
            var conn = new Neo4jDB();

            GLAM g = new GLAM("Notifications!");
            Classifier user = new Classifier(g);
            user.email = "notifyMeCreate@someplace.com";
            conn.addClassifier(user);

            Assert.AreEqual<int>(1, conn.createNotification("Testing notifications!", user.email));
        }

        [TestMethod]
        public void GetNotification_Exists()
        {
            var conn = new Neo4jDB();

            GLAM g = new GLAM("Notifications!");
            Classifier user = new Classifier(g);
            user.email = "notifyMeGetSome@someplace.com";
            conn.addClassifier(user);

            conn.createNotification("Testing GET notifications!", user.email);

            SortedDictionary<string, string> myNotifications = conn.getNotifications(user.email);

            Assert.AreEqual(1, myNotifications.Count);
            Assert.AreEqual("Testing GET notifications!", myNotifications.ElementAt(0).Value);
        }

        [TestMethod]
        public void GetNotification_NoneExists()
        {
            Assert.Fail();

            var conn = new Neo4jDB();

            GLAM g = new GLAM("Notifications!");
            Classifier user = new Classifier(g);
            user.email = "notifyMeGetNone@someplace.com";
            conn.addClassifier(user);

            SortedDictionary<string, string> noNotifications = conn.getNotifications(user.email);

            Assert.AreEqual(0,noNotifications.Count);
        }

        [TestMethod]
        public void RemoveNotification_RemoveAll()
        {
            Assert.Fail();
        }
    }
}