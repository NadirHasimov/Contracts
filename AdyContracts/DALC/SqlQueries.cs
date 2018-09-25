using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdyContracts.DALC
{
    public class SqlQueries
    {
        public static class User
        {
            public const string checkPermission = @"SELECT  id FROM tbl_user WHERE username=@username  AND global_permission=1";

            public const string addUser = @"INSERT INTO tbl_user 
            (firstname,lastname,gender,birthdate,email,username,password,
            department_id,role_id,confirmation_status)
            VALUES 
            (@firstname,@lastname,@gender,@birthdate,@email,@username,@password,
            @department_id,ISNULL(@role_id,'1'),@confirmation_status)";

            public const string insertLog = @"INSERT INTO tbl_login_log
            (username,[password],local_machine_ip,external_ip,login_success_state)
            VALUES 
            (@username,@password,@local_machine_ip,@external_ip,@login_success_state)";

            public const string addLog = @"DECLARE  @user_id INT,@operation_type INT 

                                           SELECT @user_id=id FROM tbl_user WHERE username=@username
                                           SELECT @operation_type=mnu_id FROM tbl_menus WHERE MNU_PAGE_URL=@action_name
                                           
                                           INSERT INTO tbl_logs 
                                           (user_id,user_ip,operation_type,
                                            log_success_status,description)
                                           VALUES
                                           (@user_id,@user_ip,
                                            COALESCE(@operation_type,0),@status,@description)";

            public const string checkUsername = @"SELECT COUNT(*) FROM tbl_user 
                             WHERE username=@username AND confirmation_status=1";

            public const string checkUserLogin = @"SELECT ID FROM tbl_user 
                    where username=@username AND password=@password AND confirmation_status=1";

            public const string getUserRole = @"SELECT sc_value FROM tbl_specodes 
                       INNER JOIN  tbl_user ON tbl_specodes.sc_code=tbl_user.role_id
                       WHERE username=@username and sc_type='Role'";

            public const string getMenus = @"SELECT  Replace(MNU_CAPTION2,'Operations', 
                                            (SELECT  case when MNU_CAPTION2 is null then '' else MNU_CAPTION2 end
                                             FROM  tbl_menus t1 
                                             INNER JOIN tbl_role ON tbl_role.role_mnu_id=t1.mnu_id
                                             INNER JOIN tbl_specodes ON tbl_specodes.sc_code=tbl_role.ur_id
                                             LEFT JOIN tbl_user ON tbl_role.ur_id=tbl_user.id
                                             WHERE (MNU_STATUS=0 AND (role_type_id=0 AND sc_value =@role_name) OR (username=@username AND role_type_id=1)) 
                                             AND tbl_specodes.sc_type='Role' and  t1.PARENT_ID=t.mnu_id 
                                             for xml path(''),Type).value('.','nvarchar(MAX)')
                                             ) as AZ, -- First column Azerbaijan

                                             Replace(MNU_CAPTION3,'Operations', 
                                             (SELECT  case when MNU_CAPTION3 is null then '' else MNU_CAPTION3 end
                                             FROM  tbl_menus t1 
                                              INNER JOIN tbl_role ON tbl_role.role_mnu_id=t1.mnu_id
                                              INNER JOIN tbl_specodes ON tbl_specodes.sc_code=tbl_role.ur_id
                                              LEFT JOIN tbl_user ON tbl_role.ur_id=tbl_user.id
                                              WHERE (MNU_STATUS=0 AND (role_type_id=0 AND sc_value =@role_name) OR (username=@username AND role_type_id=1)) 
                                             AND tbl_specodes.sc_type='Role' and  t1.PARENT_ID=t.mnu_id 
                                             for xml path(''),Type).value('.','nvarchar(MAX)')
                                             ) as EN -- Second column English
			                                 ,
			                                 Replace(MNU_CAPTION4,'Operations', 
                                             (SELECT  case when MNU_CAPTION4 is null then '' else MNU_CAPTION4 end
                                             FROM  tbl_menus t1 
                                              INNER JOIN tbl_role ON tbl_role.role_mnu_id=t1.mnu_id
                                              INNER JOIN tbl_specodes ON tbl_specodes.sc_code=tbl_role.ur_id
                                              LEFT JOIN tbl_user ON tbl_role.ur_id=tbl_user.id
                                              WHERE (MNU_STATUS=0 AND (role_type_id=0 AND sc_value =@role_name) OR (username=@username AND role_type_id=1)) 
                                             AND tbl_specodes.sc_type='Role' and  t1.PARENT_ID=t.mnu_id 
                                             for xml path(''),Type).value('.','nvarchar(MAX)')
                                             ) as RU -- Third column Russian
                                                                           
		                                   FROM tbl_role 
		                                   INNER JOIN tbl_menus t ON tbl_role.role_mnu_id=t.mnu_id
		                                   INNER JOIN tbl_specodes ON tbl_specodes.sc_code=tbl_role.ur_id
                                           LEFT JOIN tbl_user ON tbl_role.ur_id=tbl_user.id
                                           WHERE (MNU_STATUS=0 and (role_type_id=0 AND sc_value =@role_name) OR (username=@username AND role_type_id=1)) 
                                           AND tbl_specodes.sc_type='Role'	and PARENT_ID=0";

            public const string getRolesForMenu = @"SELECT tbl_specodes.sc_value as role_name FROM tbl_role 
                                                    INNER JOIN tbl_specodes ON
                                                    tbl_role.ur_id=tbl_specodes.sc_code AND tbl_specodes.sc_type='Role'
                                                    INNER JOIN tbl_menus ON tbl_role.role_mnu_id=tbl_menus.mnu_id
                                                    WHERE tbl_menus.MNU_PAGE_URL=@menu_name AND role_type_id=0 ";

            public const string getUsersForMenu = @"SELECT distinct(username)  FROM tbl_user
                                                    INNER JOIN tbl_role on tbl_user.id=tbl_role.ur_id AND role_type_id=1
                                                    INNER JOIN tbl_menus on tbl_menus.mnu_id=tbl_role.role_mnu_id
                                                    where tbl_menus.MNU_PAGE_URL=@menu_name";

            public const string deleteUser = "UPDATE tbl_user SET confirmation_status=0 WHERE id=@id";

            public const string getDepartments = "SELECT * FROM tbl_department";

            public const string getUsers = @"SELECT id,firstname +' '+ lastname AS [name] 
                                             FROM tbl_user 
                                             WHERE confirmation_status=1";
        }
        public static class Attachment
        {
            public const string insert = @"BEGIN TRANSACTION 
                                                BEGIN TRY
                                                    DECLARE @attachment_id INT,@depart_id int;
                                                	INSERT INTO tbl_attachment
                                                		 (file_name, gen_file_name, content_type, file_size)
                                                		 VALUES
                                                		 (@file_name, @gen_file_name, @content_type, @file_size);
                                                	
                                                        SELECT @attachment_id=SCOPE_IDENTITY();
                                                		
                                                        SELECT @depart_id=department_id FROM tbl_user WHERE username=@username

                                                        INSERT INTO tbl_document (doc_number,attachment_id,until2018,depart_id)
                                                		VALUES (@doc_number,@attachment_id,1,@depart_id)
                                                    COMMIT TRANSACTION;
                                                END TRY
                                                	BEGIN CATCH 
                                                	ROLLBACK TRANSACTION
                                                	END CATCH";

            public const string get = @"SELECT ltrim(doc_number) doc_number,file_name,registration_date,sc_value [type],tbl_document.[description],gen_file_name,type_id
                                        FROM   tbl_document 
										INNER JOIN tbl_attachment 
                                        ON tbl_document.attachment_id=tbl_attachment.attachment_id
                                        INNER JOIN tbl_specodes 
                                        ON  tbl_specodes.sc_code=tbl_document.type_id
										INNER JOIN tbl_user
										ON tbl_document.depart_id=tbl_user.department_id
										WHERE tbl_specodes.sc_type='doc_type' AND (tbl_document.until2018=@until2018 or @until2018=2) 
										AND tbl_user.username=@username AND tbl_document.status=1  
                                        ORDER BY tbl_document.id DESC";
            public const string count = @"SELECT COUNT(*) FROM tbl_document WHERE status=1";
        }
        public static class Admin
        {
            public const string selectUsers = @"SELECT u.id ,firstname + ' ' +lastname AS fullname, 
                                                          email, gender, username, depart_name, u.confirmation_status
                                                          FROM tbl_user u 
                                                          INNER JOIN tbl_department d ON u.department_id=d.ID";
            public const string updateConfirmationStatus = @"UPDATE tbl_user SET 
                                                            confirmation_status=1,sending_email_status=1,role_id=@role_id 
                                                            WHERE id=@id";
        }
        public static class Contract
        {
            public const string insert = @"INSERT INTO tbl_contract
            (contract_id, attachment_id, description, effective_date, registration_date, admission_date, type_id)
            VALUES
            (@contract_id,(SELECT IDENT_CURRENT('tbl_attachment')) , @description, @effective_date, @registration_date, @admission_date, @type_id)";

            public const string getDocTypes = "SELECT * FROM tbl_doc_type";
        }
        public static class Document
        {
            public const string terminateOrder = @"UPDATE tbl_order_paragraphs  SET  [status]=0 WHERE id=@id";

            public const string insertChangedOrders = @"INSERT INTO tbl_orders_changed_orders_mapping (order_doc_num,changed_order_doc_num)
                                                     SELECT @order_doc_num,[value] FROM @changed_orders";

            public const string getTypes = @"SELECT sc_code,sc_value 
                                             FROM tbl_specodes
                                             WHERE sc_type='doc_type' and sc_code>0";

            public const string getReceivingAuthorities = @"SELECT sc_code,sc_value 
                                                            FROM tbl_specodes 
                                                            WHERE sc_type='authority_type' and sc_code>0";
            public const string getClassTypes = @"SELECT sc_code,sc_value 
                                                  FROM tbl_specodes 
                                                  WHERE sc_type='class_type' and sc_code>0 ";

            public const string insert = @"
                                                    DECLARE @attachment_id INT,@depart_id INT;
		                                            INSERT INTO tbl_attachment
		                                             (file_name, gen_file_name, content_type, file_size)
		                                             VALUES
		                                             (@file_name, @gen_file_name, @content_type, @file_size);
	                                                
                                                    SELECT @attachment_id=SCOPE_IDENTITY();

		                                            SELECT @depart_id=department_id FROM tbl_user WHERE username=@username
                                                    
                                                     INSERT INTO tbl_document 
                                                     (doc_number, attachment_id, gov_reg_number, receiving_authority_id,
                                                      description, registration_date, type_id,until2018,word_text,termination_doc,depart_id)
                                                     VALUES
                                                     (@doc_number,@attachment_id,@gov_reg_number, 
                                                     @receiving_authority_id,
                                                     @description, @registration_date, 
                                                     @type_id,@until_2018,@word_text,@termination_doc,@depart_id)";
            public const string insertOrder = @"
                                                    DECLARE @attachment_id INT;
		                                            INSERT INTO tbl_attachment
		                                             (file_name, gen_file_name, content_type, file_size)
		                                             VALUES
		                                             (@file_name, @gen_file_name, @content_type, @file_size);
	                                                
                                                    SELECT @attachment_id=SCOPE_IDENTITY();

                                                    INSERT INTO tbl_document 
                                                     (doc_number, attachment_id, gov_reg_number, receiving_authority_id,
                                                      description, registration_date,effective_date, type_id,until2018,word_text)
                                                     VALUES
                                                     (@doc_number,@attachment_id,@gov_reg_number, 
                                                     @receiving_authority_id,
                                                     @description, @registration_date, @effective_date, 
                                                     @type_id,@until_2018,@word_text)
                                                    
                                                     IF @terminated_order_num<>'0'
		                                                INSERT INTO tbl_terminated_orders 
		                                                (order_num,terminated_order_num,entire_order_status,part_order_status) 
		                                                VALUES
		                                                (@doc_number,@terminated_order_num,@entire_order_status,@part_order_status)";

            public const string delete = @"UPDATE tbl_document SET [status]=0 WHERE doc_number=@doc_number";

            public const string update = @"
                                            IF @type_id=3
                                        	BEGIN
                                        		 UPDATE tbl_document SET
                                        		 gov_reg_number=@gov_reg_number,
                                        		 receiving_authority_id=@receiving_authority_id,
                                        		 [description]=@description,
                                        		 registration_date=@registration_date,
                                        		 [type_id]=@type_id,
                                        		 termination_doc=@termination_doc
                                        		 WHERE doc_number=@orginal_id
                                        	 END 
                                           ELSE IF @type_id=1 OR @type_id=4 OR @type_id=2
                                        	BEGIN 
                                        		UPDATE tbl_document SET
                                        			gov_reg_number=@gov_reg_number,
                                        			receiving_authority_id=@receiving_authority_id,
                                        			[description]=@description,
                                        			registration_date=@registration_date,
                                        			effective_date=@effective_date,
                                        			termination_doc=@termination_doc,
                                        			[type_id]=@type_id
                                        			WHERE doc_number=@orginal_id;
                                        		IF(@termination_doc<>'0')
                                        		BEGIN 
                                                    DECLARE @chek_terminated_order_exist INT;
                                                    SELECT @chek_terminated_order_exist=COUNT(*) FROM tbl_terminated_orders WHERE order_num=@orginal_id;
                                        			IF(@chek_terminated_order_exist=1)
                                                    	BEGIN
                                                    		UPDATE tbl_terminated_orders 
                                                    		SET terminated_order_num=@termination_doc,
                                                    			entire_order_status=@entire_order_status,
                                                    			part_order_status=@part_order_status
                                                    		WHERE order_num=@orginal_id;
                                                    	END
                                                    ELSE 
                                                    	BEGIN
                                                    		INSERT INTO tbl_terminated_orders 
                                                    		(order_num,terminated_order_num,entire_order_status,part_order_status)	
                                                    		VALUES
                                                    		(@orginal_id,@termination_doc,@entire_order_status,@part_order_status)
                                                    	END
                                        		END
                                                ELSE 
                                                    BEGIN 
                                                        DELETE FROM tbl_terminated_orders WHERE order_num=@orginal_id
                                                    END
                                        	    DELETE FROM tbl_orders_changed_orders_mapping WHERE order_doc_num=@orginal_id;
                                        		IF((SELECT TOP 1 [value] FROM @changed_orders)<>'000')
                                        			BEGIN
                                        				INSERT INTO tbl_orders_changed_orders_mapping (order_doc_num,changed_order_doc_num)
                                        				SELECT @orginal_id,[value] FROM @changed_orders
                                        			END
                                        	END";

            public const string getDocumentById = @"DECLARE @count INT,@changes INT;

                                                    SELECT @changes=count(*) from tbl_orders_changed_orders_mapping WHERE order_doc_num=@doc_number
                                                    
                                                    IF @type_id=3
                                                    	BEGIN
                                                    		SELECT a.file_name,d.*
                                                    		FROM tbl_document d
                                                    		INNER JOIN tbl_attachment a on d.attachment_id=a.attachment_id
                                                    		WHERE d.doc_number=@doc_number AND d.type_id=3
                                                    	END
                                                    ELSE IF @type_id=1 OR @type_id=4 OR @type_id=2
                                                    	BEGIN
                                                    		SELECT @count=COUNT(*) FROM tbl_terminated_orders WHERE order_num=@doc_number
                                                    		if (@count=1)
                                                    		BEGIN
                                                    			SELECT  a.file_name, d.*,
                                                    					t.order_num, t.terminated_order_num,entire_order_status,part_order_status
                                                    			FROM tbl_document d
                                                    			INNER JOIN tbl_terminated_orders t ON t.order_num=d.doc_number
                                                    			INNER JOIN tbl_attachment a on d.attachment_id=a.attachment_id
                                                    			WHERE d.doc_number=@doc_number AND d.type_id=@type_id
                                                    		END
                                                    		ELSE
                                                    		BEGIN
                                                    			SELECT a.file_name,d.*, '0' terminated_order_num
                                                    			FROM tbl_document d
                                                    			INNER JOIN tbl_attachment a on d.attachment_id=a.attachment_id
                                                    			WHERE d.doc_number=@doc_number AND d.type_id=@type_id
                                                    		END
                                                    	END
                                                    
                                                    SELECT @changes=COUNT(*) FROM tbl_orders_changed_orders_mapping t WHERE t.order_doc_num=@doc_number
                                                    
                                                    IF (@changes>0)
                                                    BEGIN
                                                    	DECLARE @changed_orders NVARCHAR(MAX) 
                                                    	SELECT @changed_orders = COALESCE(@changed_orders + ',', '') + changed_order_doc_num
                                                    	FROM tbl_orders_changed_orders_mapping where order_doc_num=@doc_number
                                                    	SELECT @changed_orders AS changed_orders
                                                    END";

            public const string insertNewReceiver = @"INSERT INTO tbl_specodes
                                                       (sc_type,sc_code,sc_value,sc_default,sc_order,sc_status)
                                                      VALUES
                                                      ('authority_type', (SELECT MAX(CAST(sc_code as int))+1 FROM tbl_specodes WHERE sc_type='authority_type') ,
                                                      @receiver, 0,
                                                       (SELECT MAX(cast(sc_order as int))+1 FROM tbl_specodes WHERE sc_type='authority_type'), 0)
                                                    SELECT sc_code FROM tbl_specodes WHERE sc_id=SCOPE_IDENTITY();";

            public const string checkDocNumber = @"SELECT COUNT(*) FROM tbl_document d 
                                                   WHERE d.doc_number=@doc_number AND d.[status]=1";

            public const string getSelectLists = @"  SELECT doc_number, doc_number + '\ ' + 
                                                     CASE 
	                                                    	WHEN LEN([description])>60 THEN SUBSTRING([description],1,50)+'...' 
	                                                       ELSE DESCRIPTION 
                                                     END text 
                                                     FROM tbl_document
                                                     WHERE tbl_document.status=1 AND (tbl_document.type_id=@type_id OR @type_id=0)";
            public const string getOrderDocs = @" SELECT doc_number, doc_number + '\ ' + 
                                                  CASE 
	                                                 	WHEN LEN([description])>50 THEN SUBSTRING([description],1,50)+'...' 
	                                                    ELSE DESCRIPTION 
                                                  END [text]
                                                  FROM tbl_document
                                                  WHERE tbl_document.status=1 AND tbl_document.type_id=@type_id";
            public const string insertParagraph = @"INSERT INTO tbl_order_paragraphs 
	                                                (order_number,paragraph_num,paragraph_text,paragraph_parent)
	                                                VALUES
	                                                (@order_number,@paragraph_number,@paragraph_text,@parent)";

            public const string getParagraphs = @"SELECT id, 
                                                  CASE 
                                                   	WHEN LEN(paragraph_text)>50 THEN paragraph_num+' '+ SUBSTRING(paragraph_text,1,50)+'...' 
                                                      ELSE paragraph_num +' '+ paragraph_text 
                                                  END [text]
                                                  FROM tbl_order_paragraphs
                                                  WHERE [status]=1 AND order_number=@order_number";

            public const string getDocumentForDescription = @"
                                                    DECLARE @count INT,@changes INT, @terminated_doc nvarchar(100), @attch_id nvarchar(100),@file_name nvarchar(300);
													
													SELECT @terminated_doc=termination_doc FROM tbl_document WHERE doc_number=@doc_number and type_id=@type_id
													SELECT @attch_id=attachment_id FROM tbl_document WHERE doc_number= @terminated_doc and type_id=@type_id
													SELECT @file_name=file_name FROM tbl_attachment WHERE attachment_id=@attch_id


                                                    SELECT @changes=count(*) from tbl_orders_changed_orders_mapping WHERE order_doc_num=@doc_number
                                                    
                                                    IF @type_id=3
                                                    	BEGIN
                                                    		SELECT a.file_name,d.*,@file_name file_name2,s.sc_value doc_type,s1.sc_value receiver
                                                    		FROM tbl_document d
                                                    		INNER JOIN tbl_attachment a on d.attachment_id=a.attachment_id
															LEFT JOIN tbl_specodes s ON d.type_id=s.sc_code AND s.sc_type='doc_type' 
															LEFT JOIN tbl_specodes s1 ON d.receiving_authority_id=s1.sc_code AND s1.sc_type='authority_type'
                                                    		WHERE d.doc_number=@doc_number AND d.type_id=@type_id
                                                    	END
                                                    ELSE IF @type_id=1 OR @type_id=4 OR @type_id=2
                                                    	BEGIN
                                                    		SELECT @count=COUNT(*) FROM tbl_terminated_orders WHERE order_num=@doc_number
                                                    		if (@count=1)
                                                    		BEGIN
                                                    			SELECT  a.file_name, d.*,@file_name file_name2,
                                                    					t.order_num, t.terminated_order_num,entire_order_status,part_order_status,s.sc_value doc_type,s1.sc_value receiver
                                                    			FROM tbl_document d
                                                    			INNER JOIN tbl_terminated_orders t ON t.order_num=d.doc_number
                                                    			INNER JOIN tbl_attachment a on d.attachment_id=a.attachment_id
																LEFT JOIN tbl_specodes s ON d.type_id=s.sc_code AND s.sc_type='doc_type' 
															    LEFT JOIN tbl_specodes s1 ON d.receiving_authority_id=s1.sc_code AND s1.sc_type='authority_type'
                                                    			WHERE d.doc_number=@doc_number AND d.type_id=@type_id
                                                    		END
                                                    		ELSE
                                                    		BEGIN
                                                    			SELECT a.file_name,d.*, '0' terminated_order_num,@file_name file_name2,s.sc_value doc_type,s1.sc_value receiver
                                                    			FROM tbl_document d
                                                    			INNER JOIN tbl_attachment a on d.attachment_id=a.attachment_id
																LEFT JOIN tbl_specodes s ON d.type_id=s.sc_code AND s.sc_type='doc_type' 
															    LEFT JOIN tbl_specodes s1 ON d.receiving_authority_id=s1.sc_code AND s1.sc_type='authority_type'
                                                    			WHERE d.doc_number=@doc_number AND d.type_id=@type_id
                                                    		END
                                                    	END
                                                    
                                                    SELECT @changes=COUNT(*) FROM tbl_orders_changed_orders_mapping t WHERE t.order_doc_num=@doc_number
                                                    
                                                    IF (@changes>0)
                                                    BEGIN
                                                    	DECLARE @changed_orders NVARCHAR(MAX) 
                                                    	SELECT @changed_orders = COALESCE(@changed_orders + ',', '') + file_name
                                                    	FROM tbl_orders_changed_orders_mapping c
														INNER JOIN tbl_document d ON c.changed_order_doc_num=d.doc_number
														INNER JOIN tbl_attachment a ON a.attachment_id=d.attachment_id
														where c.order_doc_num=@doc_number
                                                    	SELECT @changed_orders AS changed_orders
                                                    END";

            public const string filter = @"SELECT d.registration_date, d.doc_number, a.file_name,s.sc_value [type], d.receiving_authority_id, d.status,d.[description],a.gen_file_name,d.type_id
                                                FROM tbl_document d
                                                INNER JOIN tbl_attachment a
                                                ON d.attachment_id=a.attachment_id
                                                INNER JOIN tbl_specodes s
                                                ON  s.sc_code=d.type_id
                                                INNER JOIN tbl_user u
										        ON d.depart_id=u.department_id
                                                 WHERE ((
                                                	((d.doc_number=@doc_number) OR (@doc_number=''))
                                                	AND
                                                	((d.effective_date>=@effect_date1 AND d.effective_date<=@effect_date2) or (@effect_date1 is null))
                                                	AND
                                                	((d.registration_date>=@reg_date1 AND d.registration_date<=@reg_date2) or (@reg_date1 is null))
                                                	AND 
                                                	(d.gov_reg_number=@reg_gov_number or @reg_gov_number='')
                                                	AND
                                                	(d.receiving_authority_id in (SELECT * FROM @receivers) or (0 in (SELECT * FROM @receivers )))
                                                	AND
                                                	(d.type_id in (SELECT * FROM @doc_types) or (0 in (SELECT * FROM @doc_types )))
                                                	AND
                                                     (@description='' OR ((@search_type=1)and (d.description LIKE 
                                                    	CASE WHEN @exact_same=1		
                                                    		 then @description
                                                    		 else '%'+@description+'%'
                                                    			end))
                                                      OR 
                                                    ((@search_type=2) AND(d.word_text LIKE 
                                                    	CASE WHEN @exact_same=1
                                                    	then @description
                                                    	else '%'+@description+'%'
                                                    	end)))
                                                	AND (status=@status or @status=6) 
                                                ) and s.sc_type='doc_type' and u.username=@username)
                                                ORDER BY
                                                 CASE WHEN @descending_order ='1'  THEN
                                                    CASE 
                                                      WHEN @search_order = '1' THEN cast(d.registration_date as nvarchar(max))
                                                      WHEN @search_order = '2' THEN cast(d.doc_number as nvarchar(max))
                                                      WHEN @search_order = '3' THEN cast(a.file_name as nvarchar(max))
                                                      WHEN @search_order = '4' THEN cast(s.sc_value as nvarchar(max))
                                                      WHEN @search_order = '5' THEN cast(d.receiving_authority_id as nvarchar(max))
                                                      WHEN @search_order = '6' THEN cast(d.status as nvarchar(max))
                                                    END
                                                 END  desc,
                                                 CASE WHEN @descending_order='0' THEN
                                                	   CASE 
                                                	     WHEN @search_order = '1' THEN cast(d.registration_date as nvarchar(max))
                                                	     WHEN @search_order = '2' THEN cast(d.doc_number as nvarchar(max))
                                                	     WHEN @search_order = '3' THEN cast(a.file_name as nvarchar(max))
                                                	     WHEN @search_order = '4' THEN cast(s.sc_value as nvarchar(max))
                                                	     WHEN @search_order = '5' THEN cast(d.receiving_authority_id as nvarchar(max))
                                                	     WHEN @search_order = '6' THEN cast(d.status as nvarchar(max))
                                                	   END
                                                END ASC";
        }
        public static class Court
        {
            public const string getTrials = @"SELECT c.id,c.user_id, c.hearing_date, c.address_location ,
                                                     u.firstname +' '+ u.lastname AS fullname
                                              FROM tbl_court c
                                              inner join tbl_user u
                                              ON c.user_id=u.id
                                              where c.status=1";

            public const string insertTrial = @"INSERT INTO tbl_court (user_id,address_location,hearing_date) 
                                                VALUES (@user_id,@address_location,@hearing_date)";

            public const string getNotifations = @"SELECT c.id court_id,u.firstname + ' ' + u.lastname as fullname,--Name 
                                                  CASE WHEN CAST( DATEDIFF(DAY,GETDATE(),hearing_date) as nvarchar(10)) =0 THEN N'Bu gün'
                                                 							WHEN CAST( DATEDIFF(DAY,GETDATE(),hearing_date) as nvarchar(10))=1 THEN N'Sabah'
                                                 							WHEN CAST( DATEDIFF(DAY,GETDATE(),hearing_date) as nvarchar(10))=7 THEN N'1 Həftə sonra'
                                                 							ELSE CAST( DATEDIFF(DAY,GETDATE(),hearing_date) as nvarchar(10))+ N' gün sonra' END [day], -- Day
                                                  N'Hörmətli '+u.firstname + ' ' + u.lastname + N' sizin ' + convert(char(10), hearing_date, 101)--Message
                                                 + ' ' + convert(char(5), hearing_date, 108)+ N' tarixində məhkəməniz var.' as [message]
                                                 FROM tbl_court c
                                                 INNER JOIN tbl_user u on c.user_id =u.id
                                                 WHERE [status]=1  AND DATEDIFF(DAY,GETDATE(),hearing_date)>0 AND u.username=@username
                                                 ORDER BY c.hearing_date";




        }
        public static class Role
        {

            public const string insert = @" DECLARE @role_id INT
                                            
                                            INSERT INTO tbl_specodes
                                            (sc_type,sc_code,sc_value,sc_default,sc_order,sc_status)
                                           VALUES
                                           ('Role', (SELECT MAX(sc_code)+1 FROM tbl_specodes WHERE sc_type='Role') ,
                                           @role_name, 0,
                                            (SELECT MAX(sc_order)+1 FROM tbl_specodes WHERE sc_type='Role'), 0)

                                            SELECT @role_id=sc_code FROM tbl_specodes WHERE sc_type='Role' AND sc_value=@role_name

                                            INSERT INTO tbl_role (ur_id,role_mnu_id,role_type_id)
                                            SELECT @role_id,[value],0 FROM @ids";

            public const string getAllRoles = @"SELECT sc_code,sc_value
                                                FROM tbl_specodes
                                                WHERE sc_type='Role'
                                                ORDER BY sc_order";
        }
        public static class Menu
        {
            public const string getMenus = @"WITH CTE AS (
                                        	 SELECT mnu_id, mnu_caption1,CAST( mnu_caption1 as nvarchar(255)) [Path]
                                        	 FROM tbl_menus WHERE parent_id=0  AND mnu_status=0
											 UNION ALL
											 SELECT mnu_id, mnu_caption1,CAST( 'Funksiya' as nvarchar(255)) [Path]
                                        	 FROM tbl_menus WHERE parent_id is null  AND mnu_status=0
                                        	 UNION ALL
                                        	 SELECT 
                                        	 M.mnu_id,M.mnu_caption1,CAST(C.[Path]+' >> '+M.mnu_caption1 AS NVARCHAR(255))[Path]
                                        	 FROM tbl_menus M
                                        	 INNER JOIN CTE C ON C.mnu_id=M.parent_id AND mnu_status=0
                                        )
                                        SELECT * FROM CTE";
        }
    }
}