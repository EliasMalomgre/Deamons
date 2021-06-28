package be.kdg.stemtest.model.Interactors

import be.kdg.stemtest.model.db.StemTestDb
import be.kdg.stemtest.model.entity.*
import io.reactivex.*
import io.reactivex.schedulers.Schedulers
import javax.inject.Inject

class StatementDatabaseInteractor @Inject constructor(private val stemTestDb: StemTestDb,
                                                      private val identificationDatabaseInteractor: ConnectionDatabaseInteractor) {


    fun getData(index: Int): Maybe<Statement> {
        return stemTestDb.statementDao().getStatement(index)
    }

    fun saveStatement(stelling:Statement){
        stemTestDb.statementDao().insert(stelling)
    }

    fun saveAnswerOptions(answerOptions:List<AnswerOption>, statementId: Int){
        stemTestDb.answerOptionDao().insertAllAnswerOptions(answerOptions,statementId)
    }


    fun getAnswerOptions(statementId:Int): Flowable<List<AnswerOption>> {
        var testList:List<AnswerOption> = listOf()

        return stemTestDb.answerOptionDao().getAnswerOptions(statementId)
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
            .doOnSuccess { i ->
                testList = i }
            .repeatUntil {
                testList.isNotEmpty()}
    }


    fun sendAnswer(studentAnswer: StudentAnswer) : Completable{
        val studentId = identificationDatabaseInteractor.getData()
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
            .blockingGet()
            .studentId

        studentAnswer.studentSessionId=studentId
       return  stemTestDb.answerDao().insert(studentAnswer)
    }

    fun getMax(): Single<Int> {
        return stemTestDb.statementDao().getHighestStatementNumber()
    }

    fun getMaxStatementNumber(): Int {
      return stemTestDb.settingsDao().getSettings()
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
            .map { i -> i.amountOfStatements }
            .blockingGet(5)
    }

    fun getAllAnswerOptions(): Single<List<AnswerOption>> {
        return stemTestDb.answerOptionDao().getAllAnswerOptions()
    }

    fun getAllStatements(): Single<List<Statement>> {
        return stemTestDb.statementDao().getAllStatements()
    }

    fun getAllDefinitions(): Single<List<Definition>> {
        return stemTestDb.definitionDao().getDefinitions()
    }

}